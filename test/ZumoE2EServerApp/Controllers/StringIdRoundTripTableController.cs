// ----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// ----------------------------------------------------------------------------

using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.OData;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.WindowsAzure.Mobile.Service;
using ZumoE2EServerApp.DataObjects;
using ZumoE2EServerApp.Models;
using ZumoE2EServerApp.Utils;

namespace ZumoE2EServerApp.Tables
{
    public class StringIdRoundTripTableController : TableController<StringIdRoundTripTableItem>
    {
        private SDKClientTestContext Context { get; set; }

        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            this.Context = new SDKClientTestContext(Services.Settings.Name);
            this.DomainManager = new StringIdRoundTripDomainManager(Context, Request, Services);
        }

        public IQueryable<StringIdRoundTripTableItem> GetAllRoundTrips()
        {
            return Query();
        }

        public SingleResult<StringIdRoundTripTableItem> GetRoundTrip(string id)
        {
            return Lookup(id);
        }

        protected override SingleResult<StringIdRoundTripTableItem> Lookup(string id)
        {
            IQueryable<StringIdRoundTripTableItemForDB> single = LookupDB(id);
            return SingleResult.Create<StringIdRoundTripTableItem>(single.Project().To<StringIdRoundTripTableItem>());
        }

        private IQueryable<StringIdRoundTripTableItemForDB> LookupDB(string id)
        {
            IQueryable<StringIdRoundTripTableItemForDB> single = this.Context.StringIdRoundTripTableItemForDBs.Include("Complex").Include("ComplexType").Where(item => item.Id == id);
            return single;
        }

        public async Task<StringIdRoundTripTableItem> PatchRoundTrip(string id, Delta<StringIdRoundTripTableItem> patch)
        {
            StringIdRoundTripTableItemForDB originalDb = LookupDB(id).FirstOrDefault();
            if (originalDb == null)
            {
                throw new HttpResponseException(this.Request.CreateErrorResponse(HttpStatusCode.NotFound, "Item not found to patch"));
            }

            if (patch.GetChangedPropertyNames().Contains("Complex"))
            {
                // Clean out the original items.
                foreach (var ct in originalDb.Complex.ToArray())
                {
                    this.Context.ComplexForDBs.Remove(ct);
                }
            }

            if (patch.GetChangedPropertyNames().Contains("ComplexType"))
            {
                // Clean out the original items.
                foreach (var ct in originalDb.ComplexType.ToArray())
                {
                    this.Context.ComplexTypeForDBs.Remove(ct);
                }
            }

            StringIdRoundTripTableItem original = Mapper.Map<StringIdRoundTripTableItem>(originalDb);
            patch.Patch(original);
            Mapper.Map<StringIdRoundTripTableItem, StringIdRoundTripTableItemForDB>(original, originalDb);

            if (patch.GetChangedPropertyNames().Contains("Complex"))
            {
                originalDb.IsComplexNull = original.Complex == null;
                if (original.Complex != null)
                {
                    originalDb.Complex = original.Complex.Select((s, i) => new ComplexForDB()
                    {
                        Id = Guid.NewGuid().ToString(),
                        Value = s,
                        Index = i,
                    }).ToArray();
                }
            }

            if (patch.GetChangedPropertyNames().Contains("ComplexType"))
            {
                originalDb.IsComplexTypeNull = original.ComplexType == null;
                if (original.ComplexType != null)
                {
                    originalDb.ComplexType = original.ComplexType.Select((s, i) => new ComplexTypeForDB()
                    {
                        Id = Guid.NewGuid().ToString(),
                        Value = s,
                        Index = i,
                    }).ToArray();
                }
            }

            await this.Context.SaveChangesAsync();

            return Mapper.Map<StringIdRoundTripTableItem>(originalDb);
        }

        public async Task<StringIdRoundTripTableItem> PostRoundTrip(StringIdRoundTripTableItem item)
        {
            if (string.IsNullOrEmpty(item.Id))
            {
                item.Id = Guid.NewGuid().ToString();
            }

            StringIdRoundTripTableItemForDB itemForDb = Mapper.Map<StringIdRoundTripTableItemForDB>(item);
            itemForDb.IsComplexNull = item.Complex == null;
            if (item.Complex != null)
            {
                itemForDb.Complex = item.Complex.Select((s, i) => new ComplexForDB()
                {
                    Id = Guid.NewGuid().ToString(),
                    Value = s,
                    Index = i,
                }).ToArray();
            }

            itemForDb.IsComplexTypeNull = item.ComplexType == null;
            if (item.ComplexType != null)
            {
                itemForDb.ComplexType = item.ComplexType.Select((s, i) => new ComplexTypeForDB()
                {
                    Id = Guid.NewGuid().ToString(),
                    Value = s,
                    Index = i,
                }).ToArray();
            }

            this.Context.StringIdRoundTripTableItemForDBs.Add(itemForDb);
            await this.Context.SaveChangesAsync();
            return Mapper.Map<StringIdRoundTripTableItem>(itemForDb);
        }

        public async Task DeleteRoundTrip(string id)
        {
            StringIdRoundTripTableItemForDB originalDb = LookupDB(id).FirstOrDefault();
            if (originalDb == null)
            {
                throw new HttpResponseException(this.Request.CreateErrorResponse(HttpStatusCode.NotFound, "Item not found to patch"));
            }

            // Clean out the original items.
            foreach (var ct in originalDb.Complex.ToArray())
            {
                this.Context.ComplexForDBs.Remove(ct);
            }

            foreach (var ct in originalDb.ComplexType.ToArray())
            {
                this.Context.ComplexTypeForDBs.Remove(ct);
            }

            this.Context.StringIdRoundTripTableItemForDBs.Remove(originalDb);

            await this.Context.SaveChangesAsync();
        }
    }
}
