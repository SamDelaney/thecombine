﻿using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

namespace BackendFramework.ValueModels
{
    public class Users
    {
        [BsonElement("Avatar")]
        public string Avatar { get; set; }

        [BsonElement("Name")]
        public string Name { get; set; }

        [BsonElement("Email")]
        public string Email { get; set; }

        [BsonElement("OtherConnectionField")]
        public string OtherConnecitonField { get; set; }

        [BsonElement("WorkedProjects")]
        public List<string> WorkedProjects { get; set; }

        [BsonElement("Agreement")]
        public bool Agreement { get; set; }

        [BsonElement("Password")]
        public string Password { get; set; }

        [BsonElement("UserName")]
        public string UserName { get; set; }

        [BsonElement("UILang")]
        public string UILang { get; set; }
    


    }
}