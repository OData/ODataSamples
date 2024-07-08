// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

namespace ODataWebV3Tests
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Net;
    using System.Xml;
    using System.Xml.XPath;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ServiceContentTests
    {
#if DEBUG
        String nwServiceRoot = "http://localhost:32026/V3/Northwind/Northwind.svc/";
        String odServiceRoot = "http://localhost:32026/V3/(S(sessionid))/OData/OData.svc/";
#else
        String nwServiceRoot = "http://services.odata.org/V3/Northwind/Northwind.svc/";
        String odServiceRoot = "http://services.odata.org/V3/(S(sessionid))/OData/OData.svc/";
#endif

        #region Request Utils

        private Stream MakeRequest(String requestUri, string accept, string method, string body)
        {
            HttpWebRequest request = HttpWebRequest.Create(requestUri) as HttpWebRequest;
            request.Method = method;

            if (body != null)
            {
                request.ContentType = accept == "application/json;odata=minimalmetadata" ? "application/json;odata=minimalmetadata" : "*/*";
                request.ContentLength = body.Length;
                Stream s = request.GetRequestStream();
                using (StreamWriter w = new StreamWriter(s))
                {
                    w.Write(body);
                }
            }
            else
            {
                request.Accept = accept;
            }

            try
            {
                WebResponse res = request.GetResponse();
                return res.GetResponseStream();
            }
            catch (WebException ex)
            {
                if (ex.Response != null)
                {
                    String message;
                    using (StreamReader reader = new StreamReader(ex.Response.GetResponseStream()))
                    {
                        message = reader.ReadToEnd();
                    }

                    throw new InvalidOperationException(message, ex);
                }
            }

            return null;
        }

        private string GetResponseAsText(String requestUri)
        {
            return GetResponseAsText(requestUri, "application/atom+xml", "GET", null);
        }

        private string GetResponseAsText(String requestUri, string accept)
        {
            var responseAsText = GetResponseAsText(requestUri, accept, "GET", null);
            return responseAsText;
        }

        private string GetResponseAsText(String requestUri, string accept, string method, string body)
        {
            using (StreamReader reader = new StreamReader(MakeRequest(requestUri, accept, method, body)))
            {
                return reader.ReadToEnd();
            }
        }

        private XmlDocument GetResponseAsXml(String requestUri)
        {
            XmlDocument xdoc = new XmlDocument();
            xdoc.LoadXml(GetResponseAsText(requestUri, "application/atom+xml"));
            return xdoc;    
        }

        #endregion

        [TestMethod]
        public void FormatEndPointTest()
        {
            foreach (string serviceBaseUri in new string[] { odServiceRoot, nwServiceRoot })
            {
                string jsonBaseline = GetResponseAsText(serviceBaseUri + "Products", "application/json;odata=verbose");
                string jsonResult = GetResponseAsText(serviceBaseUri + "Products?$format=verbosejson");
                Assert.AreEqual(jsonResult, jsonBaseline);
            }
        }

        [TestMethod]
        [Ignore()] // Investigate why serializer reports that ReleaseDate is null
        public void EpmForOData()
        {
            XmlDocument xDoc = GetResponseAsXml(odServiceRoot + "Products");

            VerifyXPaths(xDoc, "atom:feed/atom:entry[atom:title='Bread']");
            VerifyXPaths(xDoc, "atom:feed/atom:entry[atom:summary='Whole grain bread']");

            xDoc = GetResponseAsXml(odServiceRoot + "Categories");
            VerifyXPaths(xDoc, "atom:feed/atom:entry[atom:title='Food']");

            xDoc = GetResponseAsXml(odServiceRoot + "Suppliers");
            VerifyXPaths(xDoc, "atom:feed/atom:entry[atom:title='Exotic Liquids']");            
        }

        [TestMethod]
        [Ignore()] // Investigate why serializer reports that ReleaseDate is null
        public void ODataServiceNamespace()
        {
            XmlDocument xDoc = GetResponseAsXml(odServiceRoot + "Products");
            VerifyXPaths(xDoc, "atom:feed/atom:entry/atom:category[@term='ODataDemo.Product']");
        }

        [TestMethod]
        public void UpdateComplexTypeValueToObjectProperty()
        {
            string payload = "{ \"Name\": \"Bart\", \"odata.type\": \"ODataDemo.Product\" }";
            string response = GetResponseAsText(odServiceRoot + "Products(0)", "application/json;odata=minimalmetadata", "PUT", payload);
            Assert.IsTrue(string.IsNullOrEmpty(response));
        }

        #region XML Utils

        /// <summary>Reusable name table for tests.</summary>
        private static XmlNameTable testNameTable;

        /// <summary>Reusable namespace manager for tests.</summary>
        private static XmlNamespaceManager testNamespaceManager;

        /// <summary>Reusable name table for tests.</summary>
        public static XmlNameTable TestNameTable
        {
            get
            {
                if (testNameTable == null)
                {
                    testNameTable = new NameTable();
                }

                return testNameTable;
            }
        }

        public static XmlNamespaceManager TestNamespaceManager
        {
            get
            {
                if (testNamespaceManager == null)
                {
                    testNamespaceManager = new XmlNamespaceManager(TestNameTable);

                    // Some common namespaces used by legacy tests.
                    testNamespaceManager.AddNamespace("dw", "http://schemas.microsoft.com/ado/2007/08/dataservices");
                    testNamespaceManager.AddNamespace("csdl", "http://schemas.microsoft.com/ado/2006/04/edm");
                    testNamespaceManager.AddNamespace("csdl1", "http://schemas.microsoft.com/ado/2007/05/edm");
                    testNamespaceManager.AddNamespace("csdl12", "http://schemas.microsoft.com/ado/2008/01/edm");
                    testNamespaceManager.AddNamespace("csdl2", "http://schemas.microsoft.com/ado/2008/09/edm");
                    testNamespaceManager.AddNamespace("ads", "http://schemas.microsoft.com/ado/2007/08/dataservices");
                    testNamespaceManager.AddNamespace("adsm", "http://schemas.microsoft.com/ado/2007/08/dataservices/metadata");
                    testNamespaceManager.AddNamespace("csdl", "http://schemas.microsoft.com/ado/2006/04/edm");
                    testNamespaceManager.AddNamespace("app", "http://www.w3.org/2007/app");
                    testNamespaceManager.AddNamespace("atom", "http://www.w3.org/2005/Atom");
                    testNamespaceManager.AddNamespace("edmx", "http://schemas.microsoft.com/ado/2007/06/edmx");
                    testNamespaceManager.AddNamespace("cy", "http://www.currency.org");
                    testNamespaceManager.AddNamespace("geo", "http://www.georss.org/georss");
                    testNamespaceManager.AddNamespace("ad", "http://www.address.org");
                    testNamespaceManager.AddNamespace("tmpNs", "http://tempuri.org");
                    testNamespaceManager.AddNamespace("c", "http://www.customer.org");
                }

                return testNamespaceManager;
            }
        }

        public static void VerifyXPaths(IXPathNavigable navigable, params string[] xpaths)
        {
            Assert.IsNotNull(navigable);
            Assert.IsNotNull(xpaths);
            foreach (string xpath in xpaths)
            {
                bool isTrue;
                if (xpath.StartsWith("count"))
                {
                    XPathNavigator navigator = navigable.CreateNavigator();
                    isTrue = (bool)navigator.Evaluate(xpath, TestNamespaceManager);
                }
                else
                {
                    XPathNodeIterator iterator = navigable.CreateNavigator().Select(xpath, TestNamespaceManager);
                    isTrue = iterator.Count > 0;
                }

                if (!isTrue)
                {
                    Trace.WriteLine(navigable.CreateNavigator().OuterXml);
                    Assert.Fail("The expression " + xpath + " did not find elements. The document has just been traced.");
                }
            }
        }

        #endregion
    }
}
