using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using prjSpecialTopicMvc.Attributes;


namespace prjSpecialTopicMvc.Models;

public partial class User
{
    public Guid Uid { get; set; }

    [Required(ErrorMessage = "電話必填")]
    [Display(Name = "電話")]
    [RegularExpression(@"^09\d{8}$", ErrorMessage = "電話需為09開頭共10碼")]
    public string Phone { get; set; } = null!;

    [RequiredIfCreating] // ✅ 用自訂屬性取代 Required
    [Display(Name = "密碼")]
    [MinLength(6, ErrorMessage = "密碼長度需至少6字元")]
    [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d]{6,}$", ErrorMessage = "密碼須包含英文與數字")]
    public string Password { get; set; } = null!;

    [Required(ErrorMessage = "姓名必填")]
    [Display(Name = "姓名")]
    [RegularExpression(@"^[\u4e00-\u9fa5]{2,}$", ErrorMessage = "姓名至少兩個中文字")]
    public string Name { get; set; } = null!;

    [Required(ErrorMessage = "Email必填")]
    [Display(Name = "Email")]
    [EmailAddress(ErrorMessage = "Email格式錯誤")]
    [RegularExpression(@"^[^@\s]+@([A-Za-z]+\.)+[A-Za-z]{2,}$", ErrorMessage = "@後必須為英文")]
    public string Email { get; set; } = null!;

    [Required(ErrorMessage = "性別必填")]
    [Display(Name = "性別")]
    public bool Gender { get; set; }

    [Required(ErrorMessage = "生日必填")]
    [Display(Name = "生日")]
    public DateOnly Birthday { get; set; }

    [Required(ErrorMessage = "地址必填")]
    [Display(Name = "地址")]
    [MinLength(6, ErrorMessage = "地址至少六個字元")]
    public string? Address { get; set; }
  
    [Display(Name = "註冊日期")]
    public DateTime? RegisterDate { get; set; }

    [Display(Name = "最後登入")]
    public DateTime? LastLoginDate { get; set; }

    [Display(Name = "會員狀態")]
    public byte? Status { get; set; }

    [Display(Name = "會員等級")]
    public byte? Level { get; set; }

    [Display(Name = "頭像")]
    public string? AvatarUrl { get; set; }

    [Display(Name = "作者身分")]
    public bool? IsAuthor { get; set; }

    [Display(Name = "作者狀態")]
    public byte? AuthorStatus { get; set; }

    public virtual ICollection<DonateOrder> DonateOrders { get; set; } = new List<DonateOrder>();
    public virtual ICollection<EBookOrderMain> EBookOrderMains { get; set; } = new List<EBookOrderMain>();
    public virtual ICollection<EbookPurchased> EbookPurchaseds { get; set; } = new List<EbookPurchased>();
    public virtual ICollection<ForumPost> ForumPosts { get; set; } = new List<ForumPost>();
    public virtual ICollection<LoginLog> LoginLogs { get; set; } = new List<LoginLog>();
    public virtual ICollection<PostBookmark> PostBookmarks { get; set; } = new List<PostBookmark>();
    public virtual ICollection<PostComment> PostComments { get; set; } = new List<PostComment>();
    public virtual ICollection<PostLike> PostLikes { get; set; } = new List<PostLike>();
    public virtual ICollection<Subscriber> Subscribers { get; set; } = new List<Subscriber>();
    public virtual ICollection<UsedBook> UsedBooks { get; set; } = new List<UsedBook>();
}