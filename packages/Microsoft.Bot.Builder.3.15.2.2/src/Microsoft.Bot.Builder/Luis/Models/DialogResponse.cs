// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for
// license information.
// 
// Code generated by Microsoft (R) AutoRest Code Generator 0.16.0.0
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.

namespace Microsoft.Bot.Builder.Luis.Models
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using Newtonsoft.Json;
    using Microsoft.Rest;
    using Microsoft.Rest.Serialization;

    /// <summary>
    /// The dialog response.
    /// </summary>
    public partial class DialogResponse
    {
        /// <summary>
        /// Initializes a new instance of the DialogResponse class.
        /// </summary>
        public DialogResponse() { }

        /// <summary>
        /// Initializes a new instance of the DialogResponse class.
        /// </summary>
        public DialogResponse(string prompt = default(string), string parameterName = default(string), string parameterType = default(string), string contextId = default(string), string status = default(string))
        {
            Prompt = prompt;
            ParameterName = parameterName;
            ParameterType = parameterType;
            ContextId = contextId;
            Status = status;
        }

        /// <summary>
        /// Prompt that should be asked.
        /// </summary>
        [JsonProperty(PropertyName = "prompt")]
        public string Prompt { get; set; }

        /// <summary>
        /// Name of the parameter.
        /// </summary>
        [JsonProperty(PropertyName = "parameterName")]
        public string ParameterName { get; set; }

        /// <summary>
        /// Type of the parameter.
        /// </summary>
        [JsonProperty(PropertyName = "parameterType")]
        public string ParameterType { get; set; }

        /// <summary>
        /// The context id for dialog.
        /// </summary>
        [JsonProperty(PropertyName = "contextId")]
        public string ContextId { get; set; }

        /// <summary>
        /// The dialog status. Possible values include: 'Question', 'Finished'
        /// </summary>
        [JsonProperty(PropertyName = "status")]
        public string Status { get; set; }

    }
}
