using System.ComponentModel.DataAnnotations;

namespace MvcCookieAuthSample.ViewModels
{
    public class RegisterViewModel
    {
        [Required]//�����
        [DataType(DataType.EmailAddress)]//���ݼ���Ƿ�Ϊ����
        public string Email { get; set; }

        [Required]//�����
        [DataType(DataType.Password)]//���ݼ���Ƿ�Ϊ����
        public string Password { get; set; }

        [Required]//�����
        [DataType(DataType.Password)]//���ݼ���Ƿ�Ϊ����
        public string ConfirmedPassword { get; set; }
    }
}