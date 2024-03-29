﻿﻿using Tachyon.Game.IO;

namespace Tachyon.Game.Database
{
    /// <summary>
    /// Represent a join model which gives a filename and scope to a <see cref="FileInfo"/>.
    /// </summary>
    public interface INamedFileInfo
    {
        // An explicit foreign key property isn't required but is recommended and may be helpful to have
        int FileInfoID { get; set; }

        FileInfo FileInfo { get; set; }

        string Filename { get; set; }
    }
}
