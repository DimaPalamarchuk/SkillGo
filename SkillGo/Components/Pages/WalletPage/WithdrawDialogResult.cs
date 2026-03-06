namespace SkillGo.Components.Pages.WalletPage
{
    public class WithdrawDialogResult
    {
        public decimal Amount { get; set; }
        public string Last4 { get; set; } = "";
        public string CardholderName { get; set; } = "";
    }
}