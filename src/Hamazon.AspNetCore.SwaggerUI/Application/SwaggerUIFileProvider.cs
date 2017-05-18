using System.Collections.Generic;
using System.Reflection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;

namespace Hamazon.AspNetCore.SwaggerUI.Application
{
    public class SwaggerUIFileProvider : IFileProvider
    {
        private const string StaticFilesNamespace =
            "Hamazon.AspNetCore.SwaggerUI.bower_components.swagger_ui_b.dist";
        private const string IndexResourceName =
            "Hamazon.AspNetCore.SwaggerUI.Template.index-b.html";

        private readonly Assembly _thisAssembly;
        private readonly EmbeddedFileProvider _staticFileProvider;
        private readonly IDictionary<string, string> _indexParameters;

        public SwaggerUIFileProvider(IDictionary<string, string> indexParameters)
        {
            _thisAssembly = GetType().GetTypeInfo().Assembly;
            _staticFileProvider = new EmbeddedFileProvider(_thisAssembly, StaticFilesNamespace);
            _indexParameters = indexParameters;
        }

        public IDirectoryContents GetDirectoryContents(string subpath)
        {
            return _staticFileProvider.GetDirectoryContents(subpath);
        }

        public IFileInfo GetFileInfo(string subpath)
        {
            if (subpath == "/index.html")
                return new SwaggerUIIndexFileInfo(_thisAssembly, IndexResourceName, _indexParameters);

            return _staticFileProvider.GetFileInfo(subpath);
        }

        public IChangeToken Watch(string filter)
        {
            return _staticFileProvider.Watch(filter);
        }
    }
}