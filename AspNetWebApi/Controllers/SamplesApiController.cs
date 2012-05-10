﻿using System;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Web.Http;

using MusicAlbums;
using System.Net;
using System.Xml;
using Newtonsoft.Json.Linq;

namespace AspNetWebApi.Controllers
{
    /// <summary>
    /// Sample API Controller that demonstrates various different
    /// kinds of data types that can be passed and returned
    /// </summary>
    public class SamplesApiController : ApiController
    {

        [HttpGet]
        public void ThrowException()
        {
            throw new UnauthorizedAccessException("Unauthorized Access Sucka");
        }

        [HttpGet]
        public void ThrowError()
        {

            var resp = Request.CreateResponse<ApiMessageError>(
                    HttpStatusCode.BadRequest,
                    new ApiMessageError("Your code stinks!"));
            throw new HttpResponseException(resp);
        }




        [HttpGet]
        public HttpResponseMessage ReturnAlbum(string title)
        {
            var album = new Album();
            var resp = Request.CreateResponse<Album>(HttpStatusCode.OK, album);
            return resp;
        }


        private void ThrowSafeException(string message,
                    HttpStatusCode statusCode = HttpStatusCode.BadRequest)
        {
            var errResponse = Request.CreateResponse<ApiMessageError>(statusCode,
                                        new ApiMessageError() { message = message });

            throw new HttpResponseException(errResponse);
        }

        [HttpGet]
        public string ReturnAlbumInfo([FromBody] Album album)
        {
            return album.AlbumName + " (" + album.YearReleased.ToString() + ")";
        }

        //public string  ReturnString([FromBody] string message)
        public string ReturnFormVariableString(FormDataCollection formData)
        {
            return formData.Get("message");
        }

        [HttpGet]
        public HttpResponseMessage ReturnDateTime([FromBody] DateTime time)
        {
            return Request.CreateResponse<DateTime>(HttpStatusCode.OK, time);
        }

        [HttpGet]
        public string ReturnMessageModel(MessageModel model)
        {
            return model.Message;
        }
        public class MessageModel
        {
            public string Message { get; set; }
        }


        [HttpGet]
        public string ReturnRequestBody(HttpRequestMessage request)
        {
            return request.Content.ReadAsStringAsync().Result;
        }

        [HttpPost]
        public string ReturnXmlDocument(HttpRequestMessage request)
        {
            var doc = new XmlDocument();
            doc.Load(request.Content.ReadAsStreamAsync().Result);
            return doc.DocumentElement.OuterXml;
        }

        /// <summary>
        /// Pass in an arbitrary object and parse
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [HttpPost,HttpGet]
        public string JsonValue(JObject value)
        {            
            // Dynamically parse json object
            dynamic dval = value;
            
            string val = dval.Id;

            Album album = new Album()
            {
                Id = dval.Id,
                Entered = dval.Entered
            };

            return String.Format("{0} {1:d}", album.Id, album.Entered); 
        }

        /// <summary>
        /// Demonstrates passing multiple parameters into a method
        /// </summary>
        /// <param name="album"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public string PostAlbum(JObject jsonData)
        {
            dynamic json = jsonData;
            JObject jalbum = json.Album;
            JObject juser = json.User;
            string token = json.UserToken;

            var album = jalbum.ToObject<Album>();
            var user = juser.ToObject<User>();

            return String.Format("{0} {1} {2}", album.AlbumName, user.Name, token);
        }

        [HttpPost]
        public string PostAlbum(Album album, string userToken)
        {
            
            return String.Format("{0} {1:d} {2}", album.AlbumName, album.Entered,userToken);
        }
        

        //public PostAlbumResponse PostAlbum(PostAlbumRequest request)
        //{
        //    var album = request.Album;
        //    var userToken = request.UserToken;

        //    return new PostAlbumResponse()
        //    {
        //         IsSuccess = true,
        //         Result = String.Format("{0} {1:d} {2}", album.AlbumName, album.Entered,userToken)
        //    };
        //}

    }

    public class User
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string UserToken { get; set; }
    }

    public class PostAlbumRequest
    {
        public Album Album { get; set; }
        public User User { get; set; }
        public string UserToken { get; set; }
    }

    public class PostAlbumResponse
    {
        public string Result { get; set; }
        public bool IsSuccess { get; set; }
        public string ErrorMessage { get; set; }
    }
}