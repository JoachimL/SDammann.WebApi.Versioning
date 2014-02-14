namespace SDammann.WebApi.Versioning {
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Web.Http;
    using System.Web.Http.Dispatcher;

    /// <summary>
    ///   Represents an <see cref="IHttpControllerSelector" /> implementation that supports versioning and selects an controller based on versioning by convention (namespace.Api.Version1.xxxController). The controller to invoke is determined by the "version" key in the "Accept" HTTP header.
    /// </summary>
    /// <remarks>
    /// Derived classes must implement <see cref="GetVersion"/>
    /// </remarks>
    public abstract class AcceptHeaderVersionedControllerSelectorBase : VersionedControllerSelector {

        /// <summary>
        /// Initializes a new instance of the <see cref="AcceptHeaderVersionedControllerSelector"/> class.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        protected AcceptHeaderVersionedControllerSelectorBase(HttpConfiguration configuration)
            : base(configuration, null) { }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="AcceptHeaderVersionedControllerSelector"/> class.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="defaultVersion">The version to default to if no version is present in accept header.</param>
        protected AcceptHeaderVersionedControllerSelectorBase(HttpConfiguration configuration, string defaultVersion)
            : base(configuration, defaultVersion)
        {
        }

        protected override ControllerIdentification GetControllerIdentificationFromRequest(HttpRequestMessage request) {
            if (request == null) {
                throw new ArgumentNullException("request");
            }

            // get "accept" HTTP header value
            var acceptHeader = request.Headers.Accept;
            string apiVersion = this.GetVersionFromHeader(acceptHeader);

            string controllerName = this.GetControllerNameFromRequest(request);

            return new ControllerIdentification(controllerName, apiVersion);
        }

        /// <summary>
        /// Returns the API version from the collection with accept header values. Derived classes may override this.
        /// </summary>
        /// <param name="acceptHeader"></param>
        /// <returns></returns>
        private string GetVersionFromHeader (IEnumerable<MediaTypeWithQualityHeaderValue> acceptHeader) {
            foreach (MediaTypeWithQualityHeaderValue headerValue in acceptHeader) {
                string version = this.GetVersion(headerValue);
                
                if (version != null) {
                    return version;
                }
            }

            return DefaultVersion;
        }

        /// <summary>
        /// Derived classes implement this to return an API version from the specified mime type string
        /// </summary>
        /// <param name="mimeType"></param>
        /// <returns></returns>
        protected abstract string GetVersion(MediaTypeWithQualityHeaderValue mimeType);
    }
}