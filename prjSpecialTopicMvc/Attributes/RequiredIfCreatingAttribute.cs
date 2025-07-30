using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace prjSpecialTopicMvc.Attributes
{
    public class RequiredIfCreatingAttribute : ValidationAttribute, IClientModelValidator
    {
        public override bool IsValid(object? value)
        {
            // 這裡讓它永遠通過驗證（後端我們自己處理）
            return true;
        }

        public void AddValidation(ClientModelValidationContext context)
        {
            // ✅ 移除所有前端驗證 metadata
            context.Attributes.Remove("data-val");
            context.Attributes.Remove("data-val-required");
        }
    }
}
