using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

using NgArbi.Filters;

namespace NgArbi.Controllers
{
    public class FileUploadController : ApiController
    {

        //private const Int64 CHUNK_SIZE = 1024 * 1024 * 10;

        [HttpPost]
        [Route("api/FileUpload/UploadStatus")]
        public async Task<JObject> UploadStatus()
        {
            // status check is called at the beginning of file upload
            // to check if chunks of the files are already uploaded

            var ctx = HttpContext.Current;

            JObject ret = new JObject();

            try
            {
                var infoStr = ctx.Request.Params["FileInfo"];
                JObject info = infoStr != null ? JObject.Parse(infoStr) : new JObject();

                ret.Add("FileInfo", info);
                  
                string fileName = (string)info.GetValue("name");
                //

                var rootFolder = ctx.Request.Params["RootFolder"];
                var subFolder = ctx.Request.Params["SubFolder"].TrimStart('\\');
                var ovr= ctx.Request.Params["overwrite"];
                var reup = ctx.Request.Params["reupload"];

                bool overwrite = (ovr == null ? false : Convert.ToBoolean(ovr));
                bool reupload = (reup == null ? false : Convert.ToBoolean(reup));

                var root = ctx.Server.MapPath("~/" + rootFolder);
                var temp = Path.Combine(root, "temp");

                if (subFolder == null) subFolder = "";

                string target = (subFolder == "" ? root : Path.Combine(root, subFolder));

                // create folders if still not existing
                if (!Directory.Exists(root)) Directory.CreateDirectory(root);
                if (!Directory.Exists(temp)) Directory.CreateDirectory(temp);
                if (!Directory.Exists(target)) Directory.CreateDirectory(target);

                // check existence of file in the target folder
                string filePath = Path.Combine(target, fileName);
                if (File.Exists(filePath))
                {

                    if (overwrite)
                    {
                        // this could be a call from the client where overwrite is set
                        // because file that is existing is a stray file
                        File.Delete(filePath);
                        ret.Add("deleted", true);


                    }
                    else
                    {
                        // report to client that a file already exist in the repository
                        // possible client action:
                        //    check if data record already exist linked to the 
                        //    physical file. if record exists, show information
                        //    to the client and advice to rename the new file to 
                        //    be uploaded then proceed with the upload
                        ret.Add("status", "exists");

                        // respond to client immediately
                        return ret;
                    }


                }


                // scan temporary repo path (i.e. target\<dir version of filename>)
                string tempRepo = Path.Combine(target, fileName + ".$$$");
                if (Directory.Exists(tempRepo))
                {
                    // repo folder exists, check if chunks of file are existing
                    if (reupload)
                    {
                        // clear folder
                        ret.Add("reupload", true);
                        Directory.Delete(tempRepo, true);
                    }
                    else
                    {
                        // iterate through chunk scope and see existence of files
                        string chunks = "";
                        int end = (int)info["chunks"][1]["index"];
                        for(int idx = 0; idx <= end; idx++) {
                            string fn = GetTempFile(idx);
                            if (File.Exists(Path.Combine(tempRepo,fn))) {
                                chunks += (chunks.Length != 0 ? "," : "") + idx;
                            }
                        }

                        ret.Add("chunks", chunks);

                    }
                }
                else
                {
                    // repo folder is not yet existing and therefore
                    // a new upload is to be performed
                    // Directory.CreateDirectory(tempRepo); // => this will take place during upload of the first chunk
                }

                ret.Add("status", "proceed");

            }
            catch (Exception e)
            {
                ret.Add("status", "error");
                ret.Add("message", e.Message);
            }

            return ret;

            
        }

        [HttpPost]
        [GzipCompression]
        [Route("api/FileUpload/FileStatus")]
        public async Task<JObject> FileStatus()
        {
            var ctx = HttpContext.Current;
            JObject ret = new JObject();

            string fullPath;

            var path = ctx.Request.Params["Path"];

            if(path!=null && path != "")
            {
                fullPath = ctx.Server.MapPath("~/" + path);
            }
            else
            {
                var rootFolder = ctx.Request.Params["RootFolder"];
                var subFolder = ctx.Request.Params["SubFolder"].TrimStart('\\').TrimEnd('\\');
                var fileName = ctx.Request.Params["FileName"];

                var root = ctx.Server.MapPath("~/" + rootFolder);

                fullPath = Path.Combine(root, Path.Combine(subFolder, fileName));
            }


            FileInfo fi = new FileInfo(fullPath);

            ret.Add("status", fi.Exists ? "exists" : "missing");
            ret.Add("exists", fi.Exists);
            ret.Add("path", fi.FullName);
            ret.Add("extension", fi.Extension);
            if (fi.Exists)
            {
                ret.Add("size", fi.Length);
                ret.Add("modified", fi.LastWriteTime);
            }else
            {
                // 
                string tempRepo = fi.FullName + ".$$$";
                if (Directory.Exists(tempRepo))
                {
                    // check if chunks have already been uploaded..
                    // NOTE: might not be possible without original file information
                    // where file size and chunks sizes are required to determine
                    // upload percent completion...
                }else
                {

                }
            }

            return ret;
        }

        [HttpPost]
        [Route("api/FileUpload/CheckStatus")]
        public async Task<JObject> CheckStatus()
        {
            var ctx = HttpContext.Current;
            JObject ret = new JObject();
            JArray FileInfos = new JArray();

            string part = "";

            try  
            {


                var infoStr = ctx.Request.Params["FileInfo"];
                part = "FileInfo string - passed";
                JArray infoArr = infoStr != null ? JArray.Parse(infoStr) : new JArray();

                var rootFolder = ctx.Request.Params["RootFolder"];
                part = "RootFolder string - passed";
                var subFolder = ctx.Request.Params["SubFolder"].TrimStart('\\');
                part = "SubFolder string - passed";

                var root = ctx.Server.MapPath("~/" + rootFolder);
                var temp = Path.Combine(root, "temp");

                if (subFolder == null) subFolder = "";

                string target = (subFolder == "" ? root : Path.Combine(root, subFolder));

                foreach (JObject info in infoArr)
                {

                    string fileStatus = "";
                    string fileName = (string)info.GetValue("name");
                    string tempRepo = Path.Combine(target, fileName + ".$$$");
                    string outFilePath = Path.Combine(target, fileName);

                    if (Directory.Exists(tempRepo))
                    {
                        long mergeProgress = FileMergeProgress(tempRepo);
                        if (mergeProgress != -1)
                        {
                            // file already finished uploading, now merging...
                            fileStatus = "merging";
                            info.Add("merged", mergeProgress);
                        }
                        else
                        {
                            // check avaialble chunks
                            string chunks = "";
                            long startSize = (long)info["chunks"][0]["total"];
                            long endSize = (long)info["chunks"][1]["total"];
                            long end = (long)info["chunks"][1]["index"];
                            for (int idx = 0; idx <= end; idx++)
                            {
                                string fn = GetTempFile(idx);

                                string tempPath = Path.Combine(tempRepo, fn);
                                if (File.Exists(tempPath))
                                {

                                    FileInfo fi = new FileInfo(tempPath);
                                    if (fi.Length == (idx == end ? endSize : startSize))
                                    {
                                        chunks += (chunks.Length != 0 ? "," : "") + idx;
                                    }
                                    else
                                    {
                                        // delete chunk file if size does not conform with the 
                                        // expected size as passed from the client
                                        fi.Delete();
                                    }

                                }
                                else
                                {

                                }
                            }
                            fileStatus = chunks.Length == 0 ? "new" : "resume";
                            info.Add("chunks_found", chunks);

                        }

                    }
                    else
                    {
                        // temp repo does not exist, check if output file exist and if size the same as the 
                        // file info sent from the client
                        FileInfo outfi = new FileInfo(outFilePath);
                        if (outfi.Exists) if (outfi.Length == (long)info["size"]) fileStatus = "existing";

                    }

                    info.Add("status", fileStatus);
                    FileInfos.Add(info);
                }

                ret.Add("status", "success");
            }
            catch (Exception e)
            {
                ret.Add("status", "error");
                ret.Add("message", e.Message);
            }
            ret.Add("FileInfos", FileInfos);

            return ret;

        }


        private long FileMergeProgress(string path)
        {
            return -1;
        }


        [HttpPost]
        [Route("api/FileUpload/UploadFile")]
        public async Task<JObject> UploadFile()
        {
            var ctx = HttpContext.Current;

            JObject ret = new JObject();

            try
            {
                var infoStr = ctx.Request.Params["FileInfo"];
                var chk = ctx.Request.Params["chunk"];
                JObject info = infoStr != null ? JObject.Parse(infoStr) : new JObject();
                JObject chunk = chk != null ? JObject.Parse(chk) : new JObject();

                ret.Add("FileInfo", info);

                string fileName = (string)info.GetValue("name");
                //

                var rootFolder = ctx.Request.Params["RootFolder"];
                var subFolder = ctx.Request.Params["SubFolder"].TrimStart('\\');

                var root = ctx.Server.MapPath("~/" + rootFolder);
                var temp = Path.Combine(root, "temp");

                if (subFolder == null) subFolder = "";

                string target = (subFolder == "" ? root : Path.Combine(root, subFolder));

                // create folders if still not existing
                if (!Directory.Exists(root)) Directory.CreateDirectory(root);
                if (!Directory.Exists(temp)) Directory.CreateDirectory(temp);
                if (!Directory.Exists(target)) Directory.CreateDirectory(target);

                string tempRepo = Path.Combine(target, fileName + ".$$$");
                string chunkFile = GetTempFile((int)chunk["index"]);

                if (!Directory.Exists(tempRepo)) Directory.CreateDirectory(tempRepo);

                string chunkPath = Path.Combine(tempRepo, chunkFile);
                //string outPath = Path.Combine(tempRepo, "outraw.mp4");

                //Stream strm = ctx.Request.InputStream;

                //Byte[] buffer = new byte[strm.Length];
                //strm.Read(buffer, 0, buffer.Length);

                //// Append new chunk to the video file being built...
                //String appendError = AppendAllBytes(outPath, buffer);

                var provider = new MultipartFormDataStreamProvider(temp);

                await Request.Content.ReadAsMultipartAsync(provider);

                foreach (var file in provider.FileData)
                {
                    var localFileName = file.LocalFileName;

                    File.Move(localFileName, chunkPath);
                }

                ret.Add("chunk", chunk);
                ret.Add("status", "success");
            }
            catch (Exception e)
            {
                ret.Add("status", "error");
                ret.Add("message", e.Message);
            }

            return ret;
        }

        [HttpPost]
        [Route("api/FileUpload/MergeChunks")]
        public async Task<JObject> MergeChunks()
        {
            string section = "initialization";
            var ctx = HttpContext.Current;

            JObject ret = new JObject();

            var infoStr = ctx.Request.Params["FileInfo"];
            JObject info = infoStr != null ? JObject.Parse(infoStr) : new JObject();

            var rootFolder = ctx.Request.Params["RootFolder"];
            var subFolder = ctx.Request.Params["SubFolder"].TrimStart('\\');

            var root = ctx.Server.MapPath("~/" + rootFolder);

            if (subFolder == null) subFolder = "";

            string target = (subFolder == "" ? root : Path.Combine(root, subFolder));


            ret.Add("FileInfo", info);

            string fileName = (string)info.GetValue("name");
            //

            string tempRepo = Path.Combine(target, fileName + ".$$$");
            string targetFile = Path.Combine(target, fileName);

            try
            {

                section = "check repo folder";


                if (!Directory.Exists(tempRepo))
                {
                    ret.Add("status", "missing repo");
                }
                else
                {
                    bool passed = true;
                    int end = (int)info["chunks"][1]["index"];

                    int startSize = (int)info["chunks"][0]["total"];
                    int endSize = (int)info["chunks"][1]["total"];

                    //validate chunk completion and sizes
                    section = "validate chunks";
                    for (int idx = 0; idx <= end; idx++)
                    {
                        string fn = GetTempFile(idx);
                        string chunkPath = Path.Combine(tempRepo, fn);

                        FileInfo fi = new FileInfo(chunkPath);


                        if (!fi.Exists)
                        {
                            passed = false;
                            ret.Add("message", String.Format("Chunk {0} does not exist!", chunkPath));
                            break;
                        }

                        if (idx < end)
                        {
                            // first to second to the last chunk
                            if (fi.Length != startSize)
                            {
                                ret.Add("message", String.Format("{0}: fi.Length {1} != startSize {2}", idx, fi.Length, startSize));
                                passed = false;
                                break;
                            }
                        }
                        else
                        {
                            // last chunk
                            if (fi.Length != endSize)
                            {
                                ret.Add("message", String.Format("fi.Length {0} != endSize {1}", fi.Length, endSize));
                                passed = false;
                                break;
                            }
                        }

                    }

                    section = "check if passed";
                    if (passed)
                    {
                        // perform chun merging...
                        string outFile = Guid.NewGuid().ToString() +".tmp";
                        string outPath = Path.Combine(tempRepo, outFile);
                        section = "merging files";
                        for (int idx = 0; idx <= end; idx++)
                        {
                            string fn = GetTempFile(idx);
                            string chunkPath = Path.Combine(tempRepo, fn);
                            FileInfo fi = new FileInfo(chunkPath);
                            
                            byte[] chunk = File.ReadAllBytes(chunkPath);

                            string err = AppendAllBytes(outPath, File.ReadAllBytes(chunkPath), (int)fi.Length);
                            if (err.Length != 0) section = "AppendAllBytes error: " + err;

                            //AppendAllBytes(Path.Combine(tempRepo, "out.mp4"),);
                        }

                        File.Move(outPath, targetFile);
                        Directory.Delete(tempRepo, true);

                        ret.Add("status", "success");
                    }
                    else
                    {
                        section = "not merging files";
                        ret.Add("status", "not merged");
                    }

                } // if missing repo

            }
            catch (Exception e)
            {
                ret.Add("status", "error");
                ret.Add("message", e.Message);
                ret.Add("section", section);
            }

            return ret;
        }

        private string GetTempFile(int index) {
            const string pads = "000000";
            string filename = pads + index.ToString();
            return filename.Substring(filename.Length - pads.Length) + ".$$$";
        }


        private string AppendAllBytes(string path, byte[] bytes,int appendLength=-1)
        {
            //argument-checking here.
            try
            {
                using (var stream = new FileStream(path, FileMode.Append))
                {
                    //stream.Write(bytes, 0,appendLength);
                    stream.Write(bytes, 0, appendLength == -1 ? bytes.Length: appendLength);
                }

                return "";

            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

    }
}
