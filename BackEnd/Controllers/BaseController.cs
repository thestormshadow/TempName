using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MongoDB.EntitiesManager;


namespace BackEnd.Controllers
{
    public partial class BaseController : ControllerBase
    {
        public BaseController()
        {
            new DB("mongodb://mexalabsadmin:K27GEkI6oalF7OqN@dcenter-shard-00-00-trvmd.mongodb.net:27017,dcenter-shard-00-01-trvmd.mongodb.net:27017,dcenter-shard-00-02-trvmd.mongodb.net:27017/test?ssl=true&replicaSet=DCenter-shard-0&authSource=admin&retryWrites=true&w=majority", "DBCentral");

        }
    }
}