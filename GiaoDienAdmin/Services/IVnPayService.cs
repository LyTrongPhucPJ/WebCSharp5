﻿using GiaoDienAdmin.ViewModels;

namespace GiaoDienAdmin.Services
{
    public interface IVnPayService
    {
        string CreatePaymentUrl(HttpContext context, VnPaymentRequestModel model);
        VnPaymentResponseModel PaymentExecute(IQueryCollection collections); 
    }
}
