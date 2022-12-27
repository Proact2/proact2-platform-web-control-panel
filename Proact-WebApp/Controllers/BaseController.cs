using System;
using System.Net;
using System.Net.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Proact_WebApp.Models;

namespace Proact_WebApp.Controllers {
    public class BaseController : Controller {

        private const string _successMessageKey = "Success";
        private const string _errorMessageKey = "Error";
        private const string _messageTitleKey = "MessageTitle";
        private const string _messageBodyKey = "MessageBody";

        public BaseController() {
         
        }

        protected ActionResult GetErrorView( HttpStatusCode? statusCode ) {
            ErrorViewModel errorViewModel = new ErrorViewModel {
                StatusCode = statusCode
            };
            
            return View( $"/Views/Shared/Error.cshtml", errorViewModel);
        }

        protected void AddSuccessMessage( string title, string body ) {
            HttpContext.Session.SetInt32( _successMessageKey, 1 );
            HttpContext.Session.SetString( _messageTitleKey, title );
            HttpContext.Session.SetString( _messageBodyKey, body );
        }

        protected void AddErrorMessage( string title, string body ) {
            HttpContext.Session.SetInt32( _errorMessageKey, 1 );
            HttpContext.Session.SetString( _messageTitleKey, title );
            HttpContext.Session.SetString( _messageBodyKey, body );
        }

        protected void AddGenericHttpRequestErrorMessage(
            HttpRequestException httpRequestException ) {
            AddErrorMessage(
              httpRequestException.StatusCode.ToString(),
              httpRequestException.Message );
        }

        protected void AddGenericErrorMessage() {
            AddErrorMessage(
            "An error occurred, the request could not be completed.",
            string.Empty );
        }

        protected void ShowMessage() {
            if ( ExistMessageKey( _successMessageKey ) ) {
                ViewBag.Success = HttpContext.Session
                    .GetInt32(_successMessageKey) == 1;
            }

            if ( ExistMessageKey( _errorMessageKey ) ) {
                ViewBag.Error = HttpContext.Session
                    .GetInt32( _errorMessageKey ) == 1;
            }

            if ( ExistMessageKey( _messageTitleKey ) ) {
                ViewBag.MessageTitle = HttpContext.Session
                    .GetString( _messageTitleKey );
            }

            if ( ExistMessageKey( _messageBodyKey ) ) {
                ViewBag.MessageBody = HttpContext
                    .Session.GetString( _messageBodyKey );
            }

            CleanMessages();
        }

        private bool ExistMessageKey( string key ) {
            var exist = HttpContext.Session.Get( key );
            if ( exist != null ) {
                return true;
            }
            else {
                return false;
            }
        }

        private void CleanMessages() {
            HttpContext.Session.Remove(_successMessageKey);
            HttpContext.Session.Remove( _errorMessageKey );
            HttpContext.Session.Remove( _messageTitleKey );
            HttpContext.Session.Remove( _messageBodyKey );
        }
    }
}
