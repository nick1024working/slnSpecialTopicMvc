using System;
using System.Collections.Generic;

namespace prjSpecialTopicMvc.Models;

public partial class PostImage
{
    public int PostId { get; set; }

    public byte[]? PostImage1 { get; set; }

    public bool IsMainPic { get; set; }

    public int ImageId { get; set; }

    public virtual ForumPost Post { get; set; } = null!;
}
