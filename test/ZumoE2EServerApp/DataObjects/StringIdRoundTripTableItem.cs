// ----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// ----------------------------------------------------------------------------

using Microsoft.WindowsAzure.Mobile.Service;
using System;
using System.Collections.Generic;

namespace ZumoE2EServerApp.DataObjects
{
    public class StringIdRoundTripTableItemForDB : EntityData
    {
        public StringIdRoundTripTableItemForDB()
        {
            this.Complex = new HashSet<ComplexForDB>();
            this.ComplexType = new HashSet<ComplexTypeForDB>();
        }

        public string Name { get; set; }

        public DateTime? Date { get; set; }

        public DateTime? Date1 { get; set; }

        public bool? Bool { get; set; }

        public int Integer { get; set; }

        public double? Number { get; set; }

        public ICollection<ComplexForDB> Complex { get; set; }

        public ICollection<ComplexTypeForDB> ComplexType { get; set; }
    }

    public class ComplexForDB : EntityData
    {
        public string Value { get; set; }

        public int Index { get; set; }

        public string StringIdRoundTripTableItemForDBId { get; set; }
    }

    public class ComplexTypeForDB : EntityData
    {
        public string Value { get; set; }

        public int Index { get; set; }

        public string StringIdRoundTripTableItemForDBId { get; set; }
    }

    public class StringIdRoundTripTableItem : EntityData
    {
        public string Name { get; set; }

        public DateTime? Date { get; set; }

        public DateTime? Date1 { get; set; }

        public bool? Bool { get; set; }

        public int Integer { get; set; }

        public double? Number { get; set; }

        public IEnumerable<string> Complex { get; set; }

        public IEnumerable<string> ComplexType { get; set; }
    }
}
