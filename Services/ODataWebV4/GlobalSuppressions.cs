// This file is used by Code Analysis to maintain SuppressMessage 
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given 
// a specific target and scoped to a namespace, type, member, etc.
//
// To add a suppression to this file, right-click the message in the 
// Code Analysis results, point to "Suppress Message", and click 
// "In Suppression File".
// You do not need to add suppressions to this file manually.

#region Task 1268242:Address CodeAnalysis suppressions that were added when moving to FxCop for SDL 6.0
// Given the nature of this project, consider just disabling Code Analysis
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities", Scope = "member", Target = "ODataWebV4.Northwind.NorthwindService.#SetupNorthwindDatabase()")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security.Web.Configuration", "CA3119:EnableHttpCookiesRequireSsl", MessageId = "web.config#configuration/system.web/httpCookies[@requireSSL]")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security.Web.Configuration", "CA3135:EnableRoleManagerCookieRequireSSL", MessageId = "web.config#configuration/system.web/roleManager[@cookieRequireSSL]")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security.Web.Configuration", "CA3103:EnableFormsRequireSSL", MessageId = "web.config#configuration/system.web/authentication[@mode]/forms[@requireSSL]")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security.Web.Configuration", "CA3110:EnableAnonymousIdentificationCookieRequireSSL", MessageId = "web.config#configuration/system.web/anonymousIdentification[@cookieRequireSSL]")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1309:UseOrdinalStringComparison", MessageId = "System.String.Equals(System.String,System.StringComparison)", Scope = "member", Target = "ODataWebV4.JSONPSupportInspector.#AfterReceiveRequest(System.ServiceModel.Channels.Message&,System.ServiceModel.IClientChannel,System.ServiceModel.InstanceContext)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1309:UseOrdinalStringComparison", MessageId = "System.String.StartsWith(System.String,System.StringComparison)", Scope = "member", Target = "ODataWebV4.JSONPSupportInspector.#BeforeSendReply(System.ServiceModel.Channels.Message&,System.Object)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1304:SpecifyCultureInfo", MessageId = "System.String.ToUpper", Scope = "member", Target = "ODataWebV4.Northwind.NorthwindService.#SetupNorthwindDatabase()")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1307:SpecifyStringComparison", MessageId = "System.String.StartsWith(System.String)", Scope = "member", Target = "ODataWebV4.OData.ODataService.#CreateDefaultData(DataServiceProviderV4.DSPMetadata)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.Int32.Parse(System.String)", Scope = "member", Target = "ODataWebV4.OData.ODataService.#CreateDefaultData(DataServiceProviderV4.DSPMetadata)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1309:UseOrdinalStringComparison", MessageId = "System.String.Equals(System.String,System.String,System.StringComparison)", Scope = "member", Target = "ODataWebV4.OData.ODataService.#.ctor()")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1307:SpecifyStringComparison", MessageId = "System.String.StartsWith(System.String)", Scope = "member", Target = "ODataWebV4.OData.ODataService.#.ctor()")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1309:UseOrdinalStringComparison", MessageId = "System.String.Equals(System.String,System.String,System.StringComparison)", Scope = "member", Target = "ODataWebV4.ODataWebSessionIdManager.#GetSessionID(System.Web.HttpContext)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security.Web.Configuration", "CA3114:SetCustomErrorsModeToOn", MessageId = "web.config#configuration/system.web/customErrors[@mode]")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security.Web.Configuration", "CA3120:EnableHttpCookiesHttpOnly", MessageId = "web.config#configuration/system.web/httpCookies[@httpOnlyCookies]")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security.Web.Configuration", "CA3109:SetAnonymousIdentificationCookieProtectionToAll", MessageId = "web.config#configuration/system.web/anonymousIdentification[@cookieProtection]")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security.Web.Configuration", "CA3136:DisableRoleManagerCookieSlidingExpiration", MessageId = "web.config#configuration/system.web/roleManager[@cookieSlidingExpiration]")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security.Web.Configuration", "CA3122:DisableHttpRuntimeEnableVersionHeader", MessageId = "web.config#configuration/system.web/httpRuntime[@enableVersionHeader]")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security.Web.Configuration", "CA3102:DoNotEnableCompilationDebug", MessageId = "web.config#configuration/system.web/compilation[@debug]")]
#endregion