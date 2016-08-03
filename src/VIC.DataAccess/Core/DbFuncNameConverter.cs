﻿using System;
using System.Collections.Generic;
using VIC.DataAccess.Abstraction;

namespace VIC.DataAccess.Core
{
    public class DbFuncNameConverter : IDbFuncNameConverter
    {
        private Dictionary<Type, string> _FCs = new Dictionary<Type, string>()
        {
            { typeof(long),             "GetInt64" },
            { typeof(bool),             "GetBoolean" },
            { typeof(string),           "GetString" },
            { typeof(DateTime),         "GetDateTime" },
            { typeof(decimal),          "GetDecimal"},
            { typeof(double),           "GetDouble" },
            { typeof(int),              "GetInt32"},
            { typeof(float),            "GetFloat"  },
            { typeof(short),            "GetInt16"  },
            { typeof(byte),             "GetByte"  },
            { typeof(Guid),             "GetGuid"  }
        };

        public string Convert(Type type)
        {
            var result = "GetValue";
            _FCs.TryGetValue(type.GetRealType(), out result);
            return result;
        }
    }
}