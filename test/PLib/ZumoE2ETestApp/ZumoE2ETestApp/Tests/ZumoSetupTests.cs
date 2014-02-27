﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using ZumoE2ETestApp.Framework;

namespace ZumoE2ETestApp.Tests
{
    public static class ZumoSetupTests
    {
        public static ZumoTestGroup CreateTests()
        {
            ZumoTestGroup result = new ZumoTestGroup("Tests Setup");
            result.AddTest(CreateSetupTest());

            return result;
        }

        private static ZumoTest CreateSetupTest()
        {
            return new ZumoTest("Identify enabled runtime features", async delegate(ZumoTest test)
                {
                    var client = ZumoTestGlobals.Instance.Client;
                    JToken apiResult = await client.InvokeApiAsync("runtimeInfo", HttpMethod.Get, null);
                    try
                    {
                        ZumoTestGlobals.RuntimeFeatures = JsonConvert.DeserializeObject<Dictionary<string, bool>>(JsonConvert.SerializeObject(apiResult["features"]));
                        if (apiResult["runtime"].ToString().Contains("node.js"))
                        {
                            ZumoTestGlobals.NetRuntimeEnabled = false;
                        }
                        else
                        {
                            ZumoTestGlobals.NetRuntimeEnabled = true;
                        }
                        return true;
                    }
                    catch (Exception ex)
                    {
                        test.AddLog(ex.Message);
                        return false;
                    }
                });
        }
    }
}