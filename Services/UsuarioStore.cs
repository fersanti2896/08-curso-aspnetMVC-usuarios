using ManejoPresupuesto.Models;
using Microsoft.AspNetCore.Identity;

namespace ManejoPresupuesto.Services {
    public class UsuarioStore : IUserStore<UsuarioModel>,
                                IUserEmailStore<UsuarioModel>,
                                IUserPasswordStore<UsuarioModel> {
        private readonly IUsuarioRepository usuarioRepository;

        public UsuarioStore(IUsuarioRepository usuarioRepository) {
            this.usuarioRepository = usuarioRepository;
        }

        public async Task<IdentityResult> CreateAsync(UsuarioModel user, CancellationToken cancellationToken) {
            user.Id = await usuarioRepository.CrearUsuario(user);

            return IdentityResult.Success;
        }

        public Task<IdentityResult> DeleteAsync(UsuarioModel user, CancellationToken cancellationToken) {
            throw new NotImplementedException();
        }

        public void Dispose() { }

        public async Task<UsuarioModel> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken) {
            return await usuarioRepository.BuscarUsuarioByEmail(normalizedEmail);
        }

        public Task<UsuarioModel> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task<UsuarioModel> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken) {
            return await usuarioRepository.BuscarUsuarioByEmail(normalizedUserName);
        }

        public Task<string> GetEmailAsync(UsuarioModel user, CancellationToken cancellationToken) {
            return Task.FromResult(user.Email);
        }

        public Task<bool> GetEmailConfirmedAsync(UsuarioModel user, CancellationToken cancellationToken) {
            throw new NotImplementedException();
        }

        public Task<string> GetNormalizedEmailAsync(UsuarioModel user, CancellationToken cancellationToken) {
            throw new NotImplementedException();
        }

        public Task<string> GetNormalizedUserNameAsync(UsuarioModel user, CancellationToken cancellationToken) {
            throw new NotImplementedException();
        }

        public Task<string> GetPasswordHashAsync(UsuarioModel user, CancellationToken cancellationToken) {
            return Task.FromResult(user.PasswordHash);
        }

        public Task<string> GetUserIdAsync(UsuarioModel user, CancellationToken cancellationToken) {
            return Task.FromResult(user.Id.ToString()); 
        }

        public Task<string> GetUserNameAsync(UsuarioModel user, CancellationToken cancellationToken) {
            return Task.FromResult(user.Email);
        }

        public Task<bool> HasPasswordAsync(UsuarioModel user, CancellationToken cancellationToken) {
            throw new NotImplementedException();
        }

        public Task SetEmailAsync(UsuarioModel user, string email, CancellationToken cancellationToken) {
            throw new NotImplementedException();
        }

        public Task SetEmailConfirmedAsync(UsuarioModel user, bool confirmed, CancellationToken cancellationToken) {
            throw new NotImplementedException();
        }

        public Task SetNormalizedEmailAsync(UsuarioModel user, string normalizedEmail, CancellationToken cancellationToken) {
            user.EmailNormalizado = normalizedEmail;

            return Task.CompletedTask;
        }

        public Task SetNormalizedUserNameAsync(UsuarioModel user, string normalizedName, CancellationToken cancellationToken) {
            return Task.CompletedTask;
        }

        public Task SetPasswordHashAsync(UsuarioModel user, string passwordHash, CancellationToken cancellationToken) {
            user.PasswordHash = passwordHash;

            return Task.CompletedTask;
        }

        public Task SetUserNameAsync(UsuarioModel user, string userName, CancellationToken cancellationToken) {
            throw new NotImplementedException();
        }

        public Task<IdentityResult> UpdateAsync(UsuarioModel user, CancellationToken cancellationToken) {
            throw new NotImplementedException();
        }
    }
}
