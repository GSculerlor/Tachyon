﻿using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace Tachyon.Game.Database
{
    public interface IHasPrimaryKey
    {
        [JsonIgnore]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        int ID { get; set; }
    }
}
