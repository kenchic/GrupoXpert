using GrupoXpert.Client.Models;
using GrupoXpert.Tools;
using Newtonsoft.Json;
using RestSharp;
using System.Net;

namespace GrupoXpert.Client
{
    public class AuthenticationService
    {
        private readonly RestClient route = new RestClient($"{Configuration.AppAPIUrl()}/v1/authentication");

        public AuthorizationResponse Login(AuthorizationRequest authorization)
        {
            var request = new RestRequest("authenticate", Method.Post);
            request.AddJsonBody(authorization);

            RestResponse response = route.Execute(request);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                AuthorizationResponse auth = JsonConvert.DeserializeObject<AuthorizationResponse>(response.Content);
                return auth;
            }
            return null;
        }

        public AuthorizationResponse LoginDummy(AuthorizationRequest authorization)
        {

            AuthorizationResponse auth = new AuthorizationResponse()
            {
                Message = string.Empty,                
                Token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJuYW1laWQiOiJHQUxWQVJFWiIsIm5iZiI6MTc0NDA0NTE2NiwiZXhwIjoxNzQ0MDQ1MjI2LCJpYXQiOjE3NDQwNDUxNjZ9.vMHtjrrOwv2IBpFnF_D7A8LG6-cwdiBWfP2urSZFAzw"
            };

            return auth;
        }
    }
}
