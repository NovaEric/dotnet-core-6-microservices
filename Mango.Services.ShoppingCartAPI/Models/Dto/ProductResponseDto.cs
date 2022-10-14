﻿namespace Mango.Services.ShoppingCartAPI.Models.Dto
{
    public class ProductResponseDto
    {
        public bool IsSuccess { get; set; } = true;
        public object Result { get; set; }
        public string DisplayMessage { get; set; } = "";
        public List<string> ErrorMessages { get; set; }

    }
}
