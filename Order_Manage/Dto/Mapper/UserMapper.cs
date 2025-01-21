using Order_Manage.Dto.Request;
using Order_Manage.Models;

namespace Order_Manage.Dto.Mapper
{
    public class UserMapper
    {
        public static Account ToEntity(UpdateUserRequest userRequest)
        {
            return new Account
            {
               Id = userRequest.Id,
               AccountName = userRequest.AccountName,
               Major = userRequest.Major,
               Address = userRequest.Address,
               DateOfBirth = userRequest.DateOfBirth,
               PhoneNumber = userRequest.PhoneNumber,
               Email = userRequest.Email,
            };
        }


        public static void MapUpdateRequestToEntity(UpdateUserRequest userRequest, Account account)
        {
            if (userRequest == null || account == null) return;
            account.Id=userRequest.Id;
            account.AccountName = userRequest.AccountName;
            account.Major = userRequest.Major;
            account.Address = userRequest.Address;
            account.DateOfBirth = userRequest.DateOfBirth;
            account.PhoneNumber = userRequest.PhoneNumber;
            account.Email = userRequest.Email;
        }
    }
}
