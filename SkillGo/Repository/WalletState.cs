using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using SkillGo.Data;
using SkillGo.Repository.IRepository;

namespace SkillGo.Repository
{
    public class WalletState
    {
        private readonly AuthenticationStateProvider _auth;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWalletRepository _walletRepo;

        public decimal Balance { get; private set; }
        public bool IsLoaded { get; private set; }

        public event Action? Changed;

        public WalletState(
            AuthenticationStateProvider auth,
            UserManager<ApplicationUser> userManager,
            IWalletRepository walletRepo)
        {
            _auth = auth;
            _userManager = userManager;
            _walletRepo = walletRepo;

            _auth.AuthenticationStateChanged += OnAuthChanged;
        }

        public async Task RefreshAsync()
        {
            var authState = await _auth.GetAuthenticationStateAsync();
            var principal = authState.User;

            if (principal?.Identity?.IsAuthenticated != true)
            {
                Balance = 0m;
                IsLoaded = true;
                Changed?.Invoke();
                return;
            }

            var user = await _userManager.GetUserAsync(principal);
            if (user == null)
            {
                Balance = 0m;
                IsLoaded = true;
                Changed?.Invoke();
                return;
            }

            Balance = await _walletRepo.GetBalanceAsync(user.Id);
            IsLoaded = true;
            Changed?.Invoke();
        }

        private async void OnAuthChanged(Task<AuthenticationState> _)
        {
            await RefreshAsync();
        }
    }
}