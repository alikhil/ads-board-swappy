// <copyright file="KladrClient.cs" company="Company">
//
//    Copyright (c) 2013, Anton Gubarenko
//    All rights reserved.
//
//    Redistribution and use in source and binary forms, with or without
//    modification, are permitted provided that the following conditions are met:
//        * Redistributions of source code must retain the above copyright
//          notice, this list of conditions and the following disclaimer.
//        * Redistributions in binary form must reproduce the above copyright
//          notice, this list of conditions and the following disclaimer in the
//          documentation and/or other materials provided with the distribution.
//        * Neither the name of Sergey Mudrov nor the
//          names of its contributors may be used to endorse or promote products
//          derived from this software without specific prior written permission.
//
//    THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
//    ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
//    WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
//    DISCLAIMED. IN NO EVENT SHALL ANTON GUBARENKO BE LIABLE FOR ANY
//    DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
//    (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
//    LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
//    ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
//    (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
//    SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
//
// </copyright>
// <author>Anton Gubarenko</author>
namespace Swappy_V2.Modules.KladrModule
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Globalization;
    using System.Net;
    using System.Runtime.Serialization;
    using System.Threading.Tasks;

    /// <summary>
    /// Performs operation to get address from Kladr API.
    /// </summary>
    public class KladrClient
    {
        #region Fields
        private const string _apiEndpoint = "http://kladr-api.ru/api.php?";

        /// <summary>
        /// WebClient object to make API calls.
        /// </summary>
        private WebClient _client;

        /// <summary>
        /// Clients Token.
        /// </summary>
        private string _clientToken;

        /// <summary>
        /// Clients Key.
        /// </summary>
        private string _clientKey;

        #endregion Fields

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="KladrClient"/> class.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <param name="key">The key.</param>
        public KladrClient(string token, string key)
        {
            _clientToken = token;
            _clientKey = key;
            _client = new WebClient();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="KladrClient"/> class.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <param name="key">The key.</param>
        /// <param name="client">The client object for async operations.</param>
        public KladrClient(string token, string key, WebClient client)
        {
            _clientToken = token;
            _clientKey = key;
            _client = client ?? new WebClient();
        }

        #endregion Constructor

        #region Methods

        /// <summary>
        /// Finds the address.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        public async Task<KladrResponse> FindAddress(Dictionary<string,string> parameters)
        {
            if (parameters == null)
            {
                return await invokeException(null, "Parameters dictionary is null");
            }

            ////Assigning callback
            var paramsToPost = createParametersString(parameters);
            var request = String.Format(CultureInfo.CurrentCulture, _apiEndpoint + paramsToPost);
            var uri = new Uri(request);
            Exception e = null;
            try
            {
                var response = await _client.DownloadStringTaskAsync(uri);
                var kladrResp = JsonConvert.DeserializeObject<KladrResponse>(response);
                kladrResp.InfoMessage = "OK";
                return kladrResp;
            }
            catch (Exception ex)
            {
                e = ex;
            }
            return await invokeException(e != null ? e.InnerException : null, e.Message);

        }

        /// <summary>
        /// Address validating
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns>null if doesn't validate or name of address</returns>
        public async Task<string> Check(Dictionary<string,string> parameters)
        {
            
            parameters["withParents"] = "false";
            parameters["limit"] = "1";
            
            var addresses = await FindAddress(parameters);
            var answer = addresses.result != null && addresses.result.Length == 1 ? addresses.result[0].name : null;
            return answer;
        }

        /// <summary>
        /// Creates the parameters string.
        /// </summary>
        /// <param name="values">The values.</param>
        /// <returns></returns>
        private string createParametersString(Dictionary<string, string> values)
        {
            string parametersToPost = string.Empty;

            if (values.ContainsKey("regionId"))
                parametersToPost += "&regionId=" + values["regionId"];

            if (values.ContainsKey("districtId"))
                parametersToPost += "&districtId=" + values["districtId"];

            if (values.ContainsKey("cityId"))
                parametersToPost += "&cityId=" + values["cityId"];

            if (values.ContainsKey("streetId"))
                parametersToPost += "&streetId=" + values["streetId"];

            if (values.ContainsKey("buildingId"))
                parametersToPost += "&buildingId=" + values["buildingId"];

            if (values.ContainsKey("query"))
                parametersToPost += "&query=" + values["query"];

            if (values.ContainsKey("contentType"))
                parametersToPost += "&contentType=" + values["contentType"];

            if (values.ContainsKey("withParent"))
                parametersToPost += "&withParent=" + values["withParent"];

            if (values.ContainsKey("limit"))
                parametersToPost += "&limit=" + values["limit"];

            if (values.ContainsKey("callback"))
                parametersToPost += "&callback=" + values["callback"];

            parametersToPost += "&token=" + _clientToken;
            parametersToPost += "&key=" + _clientKey;

            if (parametersToPost.Length > 1)
                if (parametersToPost.StartsWith("&"))
                    parametersToPost = parametersToPost.Substring(1);
            return parametersToPost;
        }


        /// <summary>
        /// Invokes the exceptions for all calls.
        /// </summary>
        /// <param name="e">The exception.</param>
        /// <param name="message">The message of the exception.</param>
        /// <returns>KladrResponse with errors</returns>
        private async Task<KladrResponse> invokeException(Exception e, string message)
        {
            if (e == null)
            {
                return new KladrResponse();
            }
            return await Task.FromResult(new KladrResponse(){ Error = e, InfoMessage = message });
        }

        #endregion
    }
}
