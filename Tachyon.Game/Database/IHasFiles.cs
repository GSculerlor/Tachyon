﻿using System.Collections.Generic;

namespace Tachyon.Game.Database
{
    /// <summary>
    /// A model that contains a list of files it is responsible for.
    /// </summary>
    /// <typeparam name="TFile">The model representing a file.</typeparam>
    public interface IHasFiles<TFile>
        where TFile : INamedFileInfo
    {
        List<TFile> Files { get; set; }

        string Hash { get; set; }
    }
}
