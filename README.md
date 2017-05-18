# Hamazon.Swagger.Ui

C# Code from  [Swashbuckle.AspNetCore.SwaggerUI](https://github.com/domaindrivendev/Swashbuckle.AspNetCore/tree/master/src/Swashbuckle.AspNetCore.SwaggerUI) .
Ui Code from https://github.com/jensoleg/swagger-ui

本项目基于Swashbuckle.AspNetCore.SwaggerUI 更改而来,

使用界面比较好看的 https://github.com/jensoleg/swagger-ui.

在UI库上增加了**部分语言切换**和**过时接口**标识.

fork地址:https://github.com/hemiaoio/swagger-ui

UI示例:[Auth0 api explorer](https://auth0.com/docs/api/management/v2)

参照`Swashbuckle.AspNetCore.SwaggerUI`

使用方式:

1.安装Swashbuckle.AspNetCore 相关包

```shell
Install-Package Swashbuckle.AspNetCore.Swagger
Install-Package Swashbuckle.AspNetCore.SwaggerGen
Install-Package Hamazon.AspNetCore.SwaggerUI
```

2.添加Swagger Service
Web Startup.cs

```csharp

...
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            ...
            //MVC
            services.AddMvc(options =>
            {
                options.Filters.Add(new CorsAuthorizationFilterFactory(DefaultCorsPolicyName));
            }).AddJsonOptions(options => {
                //options.SerializerSettings.ContractResolver = new NullToEmptyResolver();
            });
            ...
            //Swagger - Enable this line and the related lines in Configure method to enable swagger UI
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new Info { Title = "Demo API", Version = "v1" });
                options.DescribeAllEnumsAsStrings();
                options.DocInclusionPredicate((docName, description) => true);
                options.DocumentFilter<DynamicApiDocumentFilter>(); //定义controller名称
            });
            services.ConfigureSwaggerGen(c => {
                var xmlFilePaths = new List<string>() {
                    "Demo.WebApi.xml",
                    "Demo.Web.xml"
                    ...
                    //添加自己需要显示注释的的XML
                };
                foreach(var filePath in xmlFilePaths) {
                    var xmlFilePath = Path.Combine(PlatformServices.Default.Application.ApplicationBasePath, filePath);
                    if(File.Exists(xmlFilePath)) {
                        c.IncludeXmlComments(xmlFilePath);
                    }
                }
            });
            ...
        }
...

         public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            ...
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "defaultWithArea",
                    template: "{area}/{controller=Home}/{action=Index}/{id?}");

                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            // Enable middleware to serve generated Swagger as a JSON endpoint
            app.UseSwagger(options => {
            });
            // Enable middleware to serve swagger-ui assets (HTML, JS, CSS etc.)
            app.UseHSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "Demo API V1");
                options.InjectOnCompleteJavaScript("lang/zh-cn.js");  //语言文件
                options.InjectOnCompleteJavaScript("js/swagger.translator.js"); //语言切换
            }); //URL: /swagger
        }
...
```

3.语言切换(如果需要) `swagger.translator.js`

```javascript
(function() {
    var i = 0;
    function promise(callback) {
        if(i >= JSConfig.OnCompleteScripts.length) {
            callback();
            return;
        }
        var script = JSConfig.OnCompleteScripts[i];
        if(script === "js/swagger.translator.js") {
            callback();
        } else {
            $.getScript(script).success(function() {
                i++;
                promise(callback);
            });
        }
    };
    promise(function() {
        if(window.SwaggerTranslator) {
            window.SwaggerTranslator.translate();
        }
    });
})();
```

4.如果要显示Controller的自定义名称,使用DynamicApiDocumentFilter设置.
DynamicApiDocumentFilter.cs

```csharp
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
namespace Demo.Swagger {
    public class DynamicApiDocumentFilter: IDocumentFilter {
        public void Apply(SwaggerDocument swaggerDoc, DocumentFilterContext context) {
             Dictionary<string, string> tags = new Dictionary<string, string>();
            tags.Add("Product", "商品操作");
            tags.Add("Order", "订单操作");
            tags.Add("User", "用户操作");
            tags.Add("Session", "回话信息");
            tags.Add("Role", "角色操作");
            ...
            //自行匹配自己的Controller名字即可
            swaggerDoc.Tags = new List<Tag>();
            tags.ForEach(c => {
                swaggerDoc.Tags.Add(new Tag() {Name = c.Key, Description = c.Value});
            });
        }
    }
}
```