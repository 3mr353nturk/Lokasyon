using AWSServerless_Google_Geocoding_Api.Domain.Repositories;
using AWSServerless_Google_Geocoding_Api.Domain.Responses;
using AWSServerless_Google_Geocoding_Api.Domain.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AWSServerless_Google_Geocoding_Api.Domain.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository userRepository;

        private readonly IUnitOfWork unitOfWork;

        public UserService(IUserRepository userRepository, IUnitOfWork unitOfWork)
        {
            this.userRepository = userRepository;
            this.unitOfWork = unitOfWork;
        }

        public UserResponse FindById(int userId)
        {
            try
            {
                User user = userRepository.FindById(userId);

                if (user == null)
                {
                    return new UserResponse("Kullanıcı bulunamadı.");
                }

                return new UserResponse(user);
            }
            catch (Exception ex)
            {
                return new UserResponse($"Kullanıcı bulunurken bir hata meydana geldi1:{ex.Message}");
            }
        }

        public UserResponse FindUsernameAndPassword(string username, string password)
        {
            try
            {
                User user = userRepository.FindByUsernameandPassword(username, password);
                if (user == null)
                {
                    return new UserResponse("Kullanıcı bulunamadı.");
                }
                else
                {
                    return new UserResponse(user);
                }
            }
            catch (Exception ex)
            {
                return new UserResponse($"Kullanıcı bulunurken bir hata meydana geldi2:{ex.Message}");
            }
        }
    }
}
