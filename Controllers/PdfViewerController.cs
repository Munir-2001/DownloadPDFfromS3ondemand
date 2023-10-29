using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc;
using Syncfusion.EJ2.PdfViewer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.IO;
using System;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Configuration;
using Syncfusion.Pdf;
/*using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;*/
using System;
using System;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Text;

namespace PdfViewerWebService
{
    [Route("[controller]")]
    [ApiController]
    public class PdfViewerController : ControllerBase
    {
        private IWebHostEnvironment _hostingEnvironment;
        //Initialize the memory cache object   
        public IMemoryCache _cache;
        private IConfiguration _configuration;
        private string _accessKey;
        private string _secretKey;
        private string _bucketName;

        public object PdfTextExtractor { get; private set; }

        public PdfViewerController(IWebHostEnvironment hostingEnvironment, IMemoryCache cache, IConfiguration configuration)
        {
            _hostingEnvironment = hostingEnvironment;
            _cache = cache;
            _configuration = configuration;
            _accessKey = _configuration.GetValue<string>("AccessKey");
            _secretKey = _configuration.GetValue<string>("SecretKey");
            _bucketName = _configuration.GetValue<string>("BucketName");
        }

        [HttpPost("Load")]
        [Microsoft.AspNetCore.Cors.EnableCors("MyPolicy")]
        [Route("[controller]/Load")]
        //Post action for Loading the PDF documents   
        public async Task<IActionResult> LoadAsync([FromBody] Dictionary<string, string> jsonObject)
        {
            Console.WriteLine("Load called");
            //Initialize the PDF viewer object with memory cache object
            PdfRenderer pdfviewer = new PdfRenderer(_cache);
            MemoryStream stream = new MemoryStream();
            object jsonResult = new object();

            if (jsonObject != null && jsonObject.ContainsKey("document"))
            {
                if (bool.Parse(jsonObject["isFileName"]))
                {
                    RegionEndpoint bucketRegion = RegionEndpoint.USEast1;

                    // Configure the AWS SDK with your access credentials and other settings
                    var s3Client = new AmazonS3Client(_accessKey, _secretKey, bucketRegion);

                    string document = jsonObject["document"];

                    // Specify the document name or retrieve it from a different source
                    var response = await s3Client.GetObjectAsync(_bucketName, document);

                    Stream responseStream = response.ResponseStream;
                    responseStream.CopyTo(stream);
                    stream.Seek(0, SeekOrigin.Begin);
                }
                else
                {
                    byte[] bytes = Convert.FromBase64String(jsonObject["document"]);
                    stream = new MemoryStream(bytes);
                }
            }

            jsonResult = pdfviewer.Load(stream, jsonObject);

            return Content(JsonConvert.SerializeObject(jsonResult));
        }

        [AcceptVerbs("Post")]
        [HttpPost("Bookmarks")]
        [Microsoft.AspNetCore.Cors.EnableCors("MyPolicy")]
        [Route("[controller]/Bookmarks")]
        //Post action for processing the bookmarks from the PDF documents
        public IActionResult Bookmarks([FromBody] Dictionary<string, string> jsonObject)
        {
            //Initialize the PDF Viewer object with memory cache object
            PdfRenderer pdfviewer = new PdfRenderer(_cache);
            var jsonResult = pdfviewer.GetBookmarks(jsonObject);
            return Content(JsonConvert.SerializeObject(jsonResult));
        }

        [AcceptVerbs("Post")]
        [HttpPost("RenderPdfPages")]
        [Microsoft.AspNetCore.Cors.EnableCors("MyPolicy")]
        [Route("[controller]/RenderPdfPages")]
        //Post action for processing the PDF documents  
        public IActionResult RenderPdfPages([FromBody] Dictionary<string, string> jsonObject)
        {
            //Initialize the PDF Viewer object with memory cache object
            PdfRenderer pdfviewer = new PdfRenderer(_cache);
            object jsonResult = pdfviewer.GetPage(jsonObject);
            return Content(JsonConvert.SerializeObject(jsonResult));
        }

        [AcceptVerbs("Post")]
        [HttpPost("RenderPdfTexts")]
        [Microsoft.AspNetCore.Cors.EnableCors("MyPolicy")]
        [Route("[controller]/RenderPdfTexts")]
        //Post action for processing the PDF texts  
        public IActionResult RenderPdfTexts([FromBody] Dictionary<string, string> jsonObject)
        {
            //Initialize the PDF Viewer object with memory cache object
            PdfRenderer pdfviewer = new PdfRenderer(_cache);
            object jsonResult = pdfviewer.GetDocumentText(jsonObject);
            return Content(JsonConvert.SerializeObject(jsonResult));
        }

        [AcceptVerbs("Post")]
        [HttpPost("RenderThumbnailImages")]
        [Microsoft.AspNetCore.Cors.EnableCors("MyPolicy")]
        [Route("[controller]/RenderThumbnailImages")]
        //Post action for rendering the ThumbnailImages
        public IActionResult RenderThumbnailImages([FromBody] Dictionary<string, string> jsonObject)
        {
            //Initialize the PDF Viewer object with memory cache object
            PdfRenderer pdfviewer = new PdfRenderer(_cache);
            object result = pdfviewer.GetThumbnailImages(jsonObject);
            return Content(JsonConvert.SerializeObject(result));
        }
        [AcceptVerbs("Post")]
        [HttpPost("RenderAnnotationComments")]
        [Microsoft.AspNetCore.Cors.EnableCors("MyPolicy")]
        [Route("[controller]/RenderAnnotationComments")]
        //Post action for rendering the annotations
        public IActionResult RenderAnnotationComments([FromBody] Dictionary<string, string> jsonObject)
        {
            //Initialize the PDF Viewer object with memory cache object
            PdfRenderer pdfviewer = new PdfRenderer(_cache);
            object jsonResult = pdfviewer.GetAnnotationComments(jsonObject);
            return Content(JsonConvert.SerializeObject(jsonResult));
        }
        [AcceptVerbs("Post")]
        [HttpPost("ExportAnnotations")]
        [Microsoft.AspNetCore.Cors.EnableCors("MyPolicy")]
        [Route("[controller]/ExportAnnotations")]
        //Post action to export annotations
        public IActionResult ExportAnnotations([FromBody] Dictionary<string, string> jsonObject)
        {
            PdfRenderer pdfviewer = new PdfRenderer(_cache);
            string jsonResult = pdfviewer.ExportAnnotation(jsonObject);
            return Content(jsonResult);
        }
        [AcceptVerbs("Post")]
        [HttpPost("ImportAnnotations")]
        [Microsoft.AspNetCore.Cors.EnableCors("MyPolicy")]
        [Route("[controller]/ImportAnnotations")]
        //Post action to import annotations
        public IActionResult ImportAnnotations([FromBody] Dictionary<string, string> jsonObject)
        {
            PdfRenderer pdfviewer = new PdfRenderer(_cache);
            string jsonResult = string.Empty;
            object JsonResult;
            if (jsonObject != null && jsonObject.ContainsKey("fileName"))
            {
                string documentPath = GetDocumentPath(jsonObject["fileName"]);
                if (!string.IsNullOrEmpty(documentPath))
                {
                    jsonResult = System.IO.File.ReadAllText(documentPath);
                }
                else
                {
                    return this.Content(jsonObject["document"] + " is not found");
                }
            }
            else
            {
                string extension = Path.GetExtension(jsonObject["importedData"]);
                if (extension != ".xfdf")
                {
                    JsonResult = pdfviewer.ImportAnnotation(jsonObject);
                    return Content(JsonConvert.SerializeObject(JsonResult));
                }
                else
                {
                    string documentPath = GetDocumentPath(jsonObject["importedData"]);
                    if (!string.IsNullOrEmpty(documentPath))
                    {
                        byte[] bytes = System.IO.File.ReadAllBytes(documentPath);
                        jsonObject["importedData"] = Convert.ToBase64String(bytes);
                        JsonResult = pdfviewer.ImportAnnotation(jsonObject);
                        return Content(JsonConvert.SerializeObject(JsonResult));
                    }
                    else
                    {
                        return this.Content(jsonObject["document"] + " is not found");
                    }
                }
            }
            return Content(jsonResult);
        }

        [AcceptVerbs("Post")]
        [HttpPost("ExportFormFields")]
        [Microsoft.AspNetCore.Cors.EnableCors("MyPolicy")]
        [Route("[controller]/ExportFormFields")]
        public IActionResult ExportFormFields([FromBody] Dictionary<string, string> jsonObject)

        {
            PdfRenderer pdfviewer = new PdfRenderer(_cache);
            string jsonResult = pdfviewer.ExportFormFields(jsonObject);
            return Content(jsonResult);
        }

        [AcceptVerbs("Post")]
        [HttpPost("ImportFormFields")]
        [Microsoft.AspNetCore.Cors.EnableCors("MyPolicy")]
        [Route("[controller]/ImportFormFields")]
        public IActionResult ImportFormFields([FromBody] Dictionary<string, string> jsonObject)
        {
            PdfRenderer pdfviewer = new PdfRenderer(_cache);
            jsonObject["data"] = GetDocumentPath(jsonObject["data"]);
            object jsonResult = pdfviewer.ImportFormFields(jsonObject);
            return Content(JsonConvert.SerializeObject(jsonResult));
        }

        [AcceptVerbs("Post")]
        [HttpPost("Unload")]
        [Microsoft.AspNetCore.Cors.EnableCors("MyPolicy")]
        [Route("[controller]/Unload")]
        //Post action for unloading and disposing the PDF document resources  
        public IActionResult Unload([FromBody] Dictionary<string, string> jsonObject)
        {
            //Initialize the PDF Viewer object with memory cache object
            PdfRenderer pdfviewer = new PdfRenderer(_cache);
            pdfviewer.ClearCache(jsonObject);
            return this.Content("Document cache is cleared");
        }


        [HttpPost("Download")]
        [Microsoft.AspNetCore.Cors.EnableCors("MyPolicy")]
        [Route("[controller]/Download")]
        //Post action for downloading the PDF documents
        public IActionResult Download([FromBody] Dictionary<string, string> jsonObject)
        {
            //Initialize the PDF Viewer object with memory cache object
            PdfRenderer pdfviewer = new PdfRenderer(_cache);
            string documentBase = pdfviewer.GetDocumentAsBase64(jsonObject);
            return Content(documentBase);
        }

        [HttpPost("PrintImages")]
        [Microsoft.AspNetCore.Cors.EnableCors("MyPolicy")]
        [Route("[controller]/PrintImages")]
        //Post action for printing the PDF documents
        public IActionResult PrintImages([FromBody] Dictionary<string, string> jsonObject)
        {
            //Initialize the PDF Viewer object with memory cache object
            PdfRenderer pdfviewer = new PdfRenderer(_cache);
            object pageImage = pdfviewer.GetPrintImage(jsonObject);
            return Content(JsonConvert.SerializeObject(pageImage));
        }

        //Gets the path of the PDF document
        private string GetDocumentPath(string document)

        {

            string documentPath = string.Empty;
            if (!System.IO.File.Exists(document))
            {
                var path = _hostingEnvironment.ContentRootPath;
                if (System.IO.File.Exists(path + "/Data/" + document))
                    documentPath = path + "/Data/" + document;
            }
            else
            {
                documentPath = document;
            }
            Console.WriteLine(documentPath);
            return documentPath;
        }
        [HttpGet("GetPdfFromS3/{pdfFileName}")]
        [Microsoft.AspNetCore.Cors.EnableCors("MyPolicy")]
        [Route("[controller]/GetPdfFromS3/{pdfFileName}")]
        public async Task<IActionResult> GetPdfFromS3(string pdfFileName)
        {
            try
            {

                RegionEndpoint bucketRegion = RegionEndpoint.USEast1;

                // Configure the AWS SDK with your access credentials and other settings
                var s3Client = new AmazonS3Client(_accessKey, _secretKey, bucketRegion);

                // Specify the S3 bucket name
                var request = new GetObjectRequest
                {
                    BucketName = _bucketName,
                    Key = pdfFileName // The name of the PDF file you want to retrieve from S3
                };

                // Retrieve the PDF file from S3
                var response = await s3Client.GetObjectAsync(request);
                Console.WriteLine("Secured a response");
                // Check if the response contains data
                //if (response.ResponseStream != null && response.ContentType = "application/pdf")
                if (!(response.ResponseStream == null))
                {
                    Response.Headers.Add("Content-Disposition", "inline; filename=TME - Odoo_wellarchitected.pdf");
                    // Return the PDF file as a FileStreamResult
                    return File(response.ResponseStream, "application/pdf", pdfFileName);
                    Console.WriteLine("gotpdf");
                }
                else
                {
                    // Handle the case where the file was not found in S3
                    return NotFound($"PDF file '{pdfFileName}' not found in the S3 bucket.");
                }
            }
            catch (Exception ex)
            {
                // Handle any exceptions that may occur during the S3 operation
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }
        // GET api/values
        /*public string Get()
        {
            var pdfFileStreamResult = GetPdfFromS3("TME - Odoo_wellarchitected.pdf");
            //var t=task.GetType();
            Console.WriteLine("taskok");
            *//*            string json = JsonConvert.SerializeObject(task);
            *//*

            if (pdfFileStreamResult == null)
            {
                Console.WriteLine("FileStreamResult is null or does not contain a Stream.");
            }
            else
            {
                try
                {
                    // Create a MemoryStream to read the PDF content from the FileStreamResult
                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        pdfFileStreamResult.FileStream.CopyTo(memoryStream);

                        // Read the PDF content as a byte array
                        byte[] pdfBytes = memoryStream.ToArray();

                        // Here, you can process the PDF content as needed.
                        // You can use a PDF library to extract text or perform other operations.

                        // For example, you can display the length of the PDF content:
                        Console.WriteLine($"PDF Content Length: {pdfBytes.Length} bytes");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("An error occurred while reading the PDF: " + ex.Message);
                }
            }
            return "ok";

        }*/


        // GET api/values
        /*public async Task<string> Get()
        {
            var pdfFileName = "TME - Odoo_wellarchitected.pdf"; // Replace with your PDF file name

            try
            {
                // Retrieve the PDF file from your S3 bucket
                var pdfFileStreamResult = await GetPdfFromS3(pdfFileName);

                if (pdfFileStreamResult == null)
                {
                    return "FileStreamResult is null or does not contain a Stream.";
                }
                else
                {
                    try
                    {
                        // Create a MemoryStream to read the PDF content from the FileStreamResult
                        using (MemoryStream memoryStream = new MemoryStream())
                        {
                            await pdfFileStreamResult.FileStream.CopyToAsync(memoryStream);

                            // Read the PDF content as a byte array
                            byte[] pdfBytes = memoryStream.ToArray();

                            // Here, you can process the PDF content as needed.
                            // You can use a PDF library to extract text or perform other operations.

                            // For example, you can display the length of the PDF content:
                            Console.WriteLine($"PDF Content Length: {pdfBytes.Length} bytes");

                            // If you want to convert the PDF content to a string, you can do so:
                            string pdfContentAsString = Encoding.UTF8.GetString(pdfBytes);

                            // Now you can work with the pdfContentAsString variable, which contains the PDF content as a string.
                            Console.WriteLine("PDF Content as String:");
                            Console.WriteLine(pdfContentAsString);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("An error occurred while reading the PDF: " + ex.Message);
                    }
                }
                return "ok";
            }
            catch (Exception ex)
            {
                // Handle any exceptions that may occur during the S3 operation
                return $"An error occurred: {ex.Message}";
            }
        }*/

        /* public async Task<IActionResult> Get()
         {
             var pdfFileStreamResult = await GetPdfFromS3("TME - Odoo_wellarchitected.pdf");

             if (pdfFileStreamResult == null)
             {
                 return NotFound("PDF file not found.");
             }

             // Check if the ActionResult is of type FileStreamResult
             if (pdfFileStreamResult is FileStreamResult actualFileStreamResult)
             {
                 using (MemoryStream memoryStream = new MemoryStream())
                 {
                     // Copy the content of the PDF file to the memory stream
                     await actualFileStreamResult.FileStream.CopyToAsync(memoryStream);

                     // Read the PDF content as a byte array
                     byte[] pdfBytes = memoryStream.ToArray();

                     // Here, you can process the PDF content as needed.
                     // You can use a PDF library to extract text or perform other operations.

                     // For example, you can display the length of the PDF content:
                     Console.WriteLine($"PDF Content Length: {pdfBytes.Length} bytes");

                     // Return the PDF content as a JSON response
                     return Ok(new { PdfContent = pdfBytes });


                 }
             }
             else
             {
                 return NotFound("Invalid PDF response.");
             }





     }*/


        /*public async Task<IActionResult> Get()
        {
            var pdfFileStreamResult = await GetPdfFromS3("TME - Odoo_wellarchitected.pdf");
            
            if (pdfFileStreamResult == null)
            {
                return NotFound("PDF file not found.");
            }

            // Check if the ActionResult is of type FileStreamResult
            if (pdfFileStreamResult is FileStreamResult actualFileStreamResult)
            {
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    // Copy the content of the PDF file to the memory stream
                    await actualFileStreamResult.FileStream.CopyToAsync(memoryStream);

                    // Read the PDF content as a byte array
                    byte[] pdfBytes = memoryStream.ToArray();
                    return View("PdfView", pdfBytes);

                    // Pass the PDF content directly to the view
                    //return View("PdfView", pdfBytes);
                    return Ok(new { PdfContent = pdfBytes });
                }
            }
            else
            {
                return NotFound("Invalid PDF response.");
            }
        }*/


      
        public async Task<IActionResult> Get()
        {
            var pdfFileStreamResult = await GetPdfFromS3("TME - Odoo_wellarchitected.pdf");
            
            /*
            if (pdfFileStreamResult == null)
            {
                return NotFound("PDF file not found.");
            }

            // Check if the ActionResult is of type FileStreamResult
            if (pdfFileStreamResult is FileStreamResult actualFileStreamResult)
            {
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    // Copy the content of the PDF file to the memory stream
                    await actualFileStreamResult.FileStream.CopyToAsync(memoryStream);

                    // Read the PDF content as a byte array
                    byte[] pdfBytes = memoryStream.ToArray();

                    // Set the content type to PDF
                    Response.ContentType = "application/pdf";

                    // Optionally, provide a content disposition header to specify the file name
                    // Response.Headers.Add("Content-Disposition", "inline; filename=TME - Odoo_wellarchitected.pdf");

                    // Write the PDF bytes directly to the response
                    await Response.Body.WriteAsync(pdfBytes, 0, pdfBytes.Length);
                }

                return Ok(); // Optionally, you can return Ok or another appropriate status code.
            }
            else
            {
                return NotFound("Invalid PDF response.");
            }*/
            //This will display in a pdf format^^^^

            
                        if (pdfFileStreamResult == null)
                        {
                            return NotFound("PDF file not found.");
                        }

                        // Check if the ActionResult is of type FileStreamResult
                        if (pdfFileStreamResult is FileStreamResult actualFileStreamResult)
                        {
                            using (MemoryStream memoryStream = new MemoryStream())
                            {
                                // Copy the content of the PDF file to the memory stream
                                await actualFileStreamResult.FileStream.CopyToAsync(memoryStream);

                                // Read the PDF content as a byte array
                                byte[] pdfBytes = memoryStream.ToArray();
                                string decoded = Encoding.UTF8.GetString(memoryStream.ToArray());
                    /*Document doc = new Document(PageSize.LETTER);
                    string outputPath = "output.pdf";
                    doc.Open();

                    PdfWriter writer = PdfWriter.GetInstance(doc, new FileStream(outputPath, FileMode.Create));
                    //string content = @"%PDF-1.3 %     15 0 obj << /Type /ExtGState /ca 1 /CA 1 >> endobj 14 0 obj << /Type /Page /Parent 1 0 R /MediaBox [0 0 612 792] /Contents 12 0 R /Resources 13 0 R >> endobj 13 0 obj << /ProcSet [/PDF /Text /ImageB /ImageC /ImageI] /ExtGState << /Gs1 15 0 R >> /XObject << /I1 5 0 R /I7 11 0 R >> /Font << /F1 16 0 R /F2 17 0 R >> >> endobj 12 0 obj << /Length 793 /Filter /FlateDecode >> stream x  WMo?1?   ?    SB{  ? @ !? &A?";

                    // Create a Paragraph to hold the content (you can use other elements like PdfPTable for more complex content)
                    Paragraph paragraph = new Paragraph(decoded);

                    // Add the Paragraph to the document
                    doc.Add(paragraph);

                    // Close the document
                    doc.Close();

                    Console.WriteLine("PDF created successfully at " + outputPath);
*/



                    /*byte[] pdfBytes = new byte[] { };*/

                    string filePath = "output.pdf";

                    try
                    {
                        // Create a FileStream and write the PDF bytes to the file
                        using (FileStream fs = new FileStream(filePath, FileMode.Create))
                        {
                            fs.Write(pdfBytes, 0, pdfBytes.Length);
                        }

                        Console.WriteLine("PDF file saved successfully at: " + filePath);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error: " + ex.Message);
                    }

                    // Pass the PDF content to a view for rendering
                    return Ok(new { PdfContent = pdfBytes });
                    //return Ok(decoded);
                            }
                        }
                        else
                        {
                            return NotFound("Invalid PDF response.");
                        }
                        }

      



        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }
    }
}