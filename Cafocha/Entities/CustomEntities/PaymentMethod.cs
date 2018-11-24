namespace Cafocha.Entities
{
    public enum PaymentMethod
    {
        All = -1,
        Cash,               // trả tiền mặt
        Cheque,             // trả sec
        Deferred,           // trả sau
        International,      // thanh toán quốc tế (VISA, MasterCard)
        Credit,             // tín dụng nói chung
        OnAcount
    }
}
