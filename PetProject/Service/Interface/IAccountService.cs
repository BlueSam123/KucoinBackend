using PetProject.Model.DTO;

namespace PetProject.Service.Interface
{
    public interface IAccountService
    {
        ResponseDto<string> LoginUser2(Model2 req);
        Task<ResponseDto<string>> RegisterUser(SignUp signUp, string Role);

        Task<ResponseDto<string>> LoginUser(SignInModel signIn);
        Task<ResponseDto<string>> ForgotPassword(string UserEmail);
        Task<ResponseDto<string>> ConfirmEmailAsync(int token, string email);

        Task<ResponseDto<string>> ResetUserPassword(ResetPassword resetPassword);
    }
}
