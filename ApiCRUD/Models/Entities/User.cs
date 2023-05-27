using ApiCRUD.Models.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace ApiCRUD.Models.Entities
{
    public class User
    {
        private const string LATIN_NUMBERS_PATTERN = "^[A-Za-z0-9]*$";
        private const string LATIN_RUSSIAN_PATTERN = "^[A-Za-zА-Яа-я]*$";
        private const string PATTERN_ERROR = "The field contains invalid characters";

        #region Guid
        /// <summary>
        /// Уникальный идентификатор пользователя.
        /// </summary>
        [Key]
        public Guid Guid { get; set; }
        #endregion

        #region Login
        /// <summary>
        /// Уникальный логин пользователя.
        /// </summary>
        [RegularExpression(LATIN_NUMBERS_PATTERN, ErrorMessage = PATTERN_ERROR)]
        public string Login { get; set; }
        #endregion

        #region Password
        /// <summary>
        /// Пароль пользователя.
        /// </summary>
        [RegularExpression(LATIN_NUMBERS_PATTERN, ErrorMessage = PATTERN_ERROR)]
        public string Password { get; set; }
        #endregion

        #region Name
        /// <summary>
        /// Имя пользователя.
        /// </summary>
        [RegularExpression(LATIN_RUSSIAN_PATTERN, ErrorMessage = PATTERN_ERROR)]
        public string Name { get; set; }
        #endregion

        #region Gender
        /// <summary>
        /// Пол пользователя.
        /// </summary>
        public Gender Gender { get; set; }
        #endregion

        #region BirthDay
        /// <summary>
        /// Дата рождения пользователя.
        /// </summary>
        public DateTime? BirthDate { get; set; }
        #endregion

        #region Admin
        /// <summary>
        /// Является ли пользователь админом.
        /// </summary>
        public bool Admin { get; set; }
        #endregion

        #region CreatedOn 
        /// <summary>
        /// Дата создания пользователя.
        /// </summary>
        public DateTime CreatedOn { get; set; }
        #endregion

        #region CreatedBy
        /// <summary>
        /// Логин Пользователя, от имени которого этот пользователь создан.
        /// </summary>
        public string CreatedBy { get; set; }
        #endregion

        #region ModifiedOn
        /// <summary>
        /// Дата изменения пользователя.
        /// </summary>
        public DateTime? ModifiedOn { get; set; }
        #endregion

        #region ModifiedBy
        /// <summary>
        /// Логин Пользователя, от имени которого этот пользователь изменён.
        /// </summary>
        public string ModifiedBy { get; set; } = null;
        #endregion

        #region RevokedOn
        /// <summary>
        /// Дата удаления пользователя.
        /// </summary>
        public DateTime? RevokedOn { get; set; }
        #endregion

        #region RevokedBy
        /// <summary>
        /// Логин Пользователя, от имени которого этот пользователь удалён.
        /// </summary>
        public string RevokedBy { get; set; } = null;
        #endregion
    }
}