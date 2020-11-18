using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace jwtToken_2.Models
{
    public class UserLogin
    {
        public string Username { get; set; }
        public string Password { get; set; }
        
    }

}
