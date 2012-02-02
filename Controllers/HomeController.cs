using System;
using System.Configuration;
using System.IO;
using System.Web.Mvc;
using Amazon.S3;
using Amazon.S3.Model;
using MVC3PluploadToAmazonS3.Helpers;
using MVC3PluploadToAmazonS3.Models;

namespace MVC3PluploadToAmazonS3.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/

        public ActionResult Index()
        {
            string acl = "private";
            string bucket = "-----------";
            string accessKeyId = "----------------";
            string secret = "--------------------";
            string policy = AmazonS3Helper.ConstructPolicy(bucket, DateTime.UtcNow.Add(new TimeSpan(0, 10, 0, 0)), acl, accessKeyId);
            string signature = AmazonS3Helper.CreateSignature(policy, secret);

            var model = new PluploadAmazonS3Model()
            {
                AWSAccessKeyId = accessKeyId,
                Policy = policy,
                Signature = signature,
                Bucket = bucket,
                Acl = acl
            };

            return View(model);
        }

        public ActionResult Interno()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Upload(int? chunk, string name)
        {
            try
            {
                var fileUpload = Request.Files[0];
                if (fileUpload == null)
                    return Content("Error", "text/plain");

                var uploadPath = Server.MapPath("~/App_Data/Uploads");
                chunk = chunk ?? 0;

                //write chunk to disk.  
                string uploadedFilePath = Path.Combine(uploadPath, name);
                if (System.IO.File.Exists(uploadedFilePath))
                    return Content("Ignored", "text/plain");

                using (var fs = new FileStream(uploadedFilePath, chunk == 0 ? FileMode.Create : FileMode.Append))
                {
                    //do something here

                    //save object in S3
                    SaveObjectInAws(fs, name);
                }

                return Content("Success", "text/plain");
            }
            catch (Exception)
            {
                return Content("Error", "text/plain");
            }
        }

        static void SaveObjectInAws(Stream pObject, string keyname)
        {
            try
            {
                using (var client = Amazon.AWSClientFactory.CreateAmazonS3Client())
                {
                    // simple object put
                    PutObjectRequest request = new PutObjectRequest();
                    request.WithBucketName(ConfigurationManager.AppSettings["bucketname"]).WithKey(keyname).WithInputStream(pObject);

                    using (client.PutObject(request)) { }
                }
            }
            catch (AmazonS3Exception amazonS3Exception)
            {
                if (amazonS3Exception.ErrorCode != null &&
                    (amazonS3Exception.ErrorCode.Equals("InvalidAccessKeyId") ||
                    amazonS3Exception.ErrorCode.Equals("InvalidSecurity")))
                {
                    Console.WriteLine("Please check the provided AWS Credentials.");
                    Console.WriteLine("If you haven't signed up for Amazon S3, please visit http://aws.amazon.com/s3");
                    throw;
                }

                Console.WriteLine("An error occurred with the message '{0}' when writing an object", amazonS3Exception.Message);
                throw;
            }
        }

    }
}
