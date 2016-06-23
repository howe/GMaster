using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Net;
using System.IO;

namespace GMLib
{
    public class HttpClient
    {
        static Encoding encoding = Encoding.UTF8;
        static string cv = "204";

        public static byte[] get(string url)
        {
            byte[] ret = null;
            HttpWebRequest request = null;
            HttpWebResponse response = null;

            try
            {
                request = createHttpRequest(url, "GET");
                response = request.GetResponse() as HttpWebResponse;

                using (Stream stream = response.GetResponseStream())
                {
                    ret = new byte[response.ContentLength];
                    stream.Read(ret, 0, ret.Length);
                }
            }
            catch (Exception e)
            {
            }
            finally
            {
                if (request != null)
                {
                    request.Abort();
                }
                if (response != null)
                {
                    response.Close();
                }
            }

            return ret;
        }

        public static string post(string url, string body)
        {

            string ret = null;
            HttpWebRequest request = null;
            HttpWebResponse response = null;

            try
            {
                request = createHttpRequest(url, "POST");
                if (body != null)
                {
                    byte[] data = encoding.GetBytes(body);
                    using (Stream stream = request.GetRequestStream())
                        stream.Write(data, 0, data.Length);
                }

                response = request.GetResponse() as HttpWebResponse;
                using (Stream stream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(stream, encoding);
                    ret = reader.ReadToEnd();
                }

            }
            catch (Exception e)
            {
            }
            finally
            {
                if (request != null)
                {
                    request.Abort();
                }
                if (response != null)
                {
                    response.Close();
                }
            }


            return ret;
        }

        public static bool download(string url, string targetPath, int size, string hash)
        {
            if (url == null || targetPath == null || size < 1 || hash == null)
                return false;

            Stream stream = null;
            Stream responseStream = null;
            HttpWebRequest request = null;
            HttpWebResponse response = null;
            
            try
            {
                request = createHttpRequest(url, "GET");
                response = request.GetResponse() as HttpWebResponse;
                responseStream = response.GetResponseStream();
                stream = new FileStream(targetPath, FileMode.Create);
                byte[] bArr = new byte[1024];
                int length = responseStream.Read(bArr, 0, (int)bArr.Length);
                while (length > 0)
                {
                    stream.Write(bArr, 0, length);
                    length = responseStream.Read(bArr, 0, (int)bArr.Length);
                }
            }
            catch (Exception e)
            {
            } 
            finally
            {
                if (stream != null)
                {
                    stream.Close();
                }
                if (responseStream != null)
                {
                    responseStream.Close();
                }
                if (request != null)
                {
                    request.Abort();
                }
                if (response != null)
                {
                    response.Close();
                }
            }

            if (!hash.Equals(getSHA1HashFromFile(targetPath)))
            {
                System.IO.File.Delete(targetPath);
            }

            return File.Exists(targetPath) && new FileInfo(targetPath).Length == size;
        }

        private static HttpWebRequest createHttpRequest(string url, string method)
        {
            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
            request.Headers["cv"] = cv;
            request.KeepAlive = false;
            request.ContentType = "application/text;";
            request.Method = method;

            return request;
        }

        public static string getSHA1HashFromFile(string fileName)
        {
            FileStream file = null;
            try
            {
                file = new FileStream(fileName, FileMode.Open);
                System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
                byte[] retVal = md5.ComputeHash(file);
                file.Close();

                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < retVal.Length; i++)
                {
                    sb.Append(retVal[i].ToString("x2"));
                }
                return sb.ToString();
            }
            catch (Exception ex)
            {
            }
            finally
            {
                if (file != null)
                    file.Close();
            }

            return null;
        }
    }


    public class QueryParam
    {
        private Hashtable param = new Hashtable();

        private QueryParam()
        {
        }

        public static QueryParam create(string key, object value)
        {
            return new QueryParam().add(key, value);
        }

        public QueryParam add(string key, object value)
        {
            param.Add(key, value);
            return this;
        }

        public string catQueryString(string url)
        {
            if (!url.EndsWith("?"))
            {
                url = url + "?";
            }

            foreach (string k in param.Keys)
            {
                url = url + k + "=" + param[k].ToString() + "&";
            }

            return url;
        }
    }
}
