using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RVMCore.TelgeramBot.Apis.Objects
{
    /// <summary>
    /// Contains information about documents or other Telegram Passport elements shared with the bot by the user.
    /// </summary>
    public class EncryptedPassportElement
    {
        /// <summary>
        /// Element type. 
        /// </summary>
        public ElementType type { get; set; }
        /// <summary>
        /// Optional. Base64-encoded encrypted Telegram Passport element data provided by the user, <para/>available for <see cref="ElementType.personal_details"/>, <see cref="ElementType.passport"/>, <see cref="ElementType.driver_license"/>, <see cref="ElementType.identity_card"/>, <see cref="ElementType.internal_passport"/> and <see cref="ElementType.address"/>. <para/>Can be decrypted and verified using the accompanying <see cref="EncryptedCredentials"/>..
        /// </summary>
        public string data { get; set; }
        /// <summary>
        /// Optional. User's verified phone number, available only for <see cref="ElementType.phone_number"/>.
        /// </summary>
        public string phone_number { get; set; }
        /// <summary>
        /// Optional. User's verified email address, available only for <see cref="ElementType.email"/>.
        /// </summary>
        public string email { get; set; }
        /// <summary>
        /// Optional. Array of encrypted files with documents provided by the user, <para/>available for <see cref="ElementType.utility_bill"/>, <see cref="ElementType.bank_statement"/>, <see cref="ElementType.rental_agreement"/>, <see cref="ElementType.passport_registration"/> and <see cref="ElementType.temporary_registration"/>. <para/>Files can be decrypted and verified using the accompanying <see cref="EncryptedCredentials"/>.
        /// </summary>
        public List<PassportFile> files { get; set; }
        /// <summary>
        /// 	Optional. Encrypted file with the front side of the document, provided by the user. <para/>Available for <see cref="ElementType.passport"/>, <see cref="ElementType.driver_license"/>, <see cref="ElementType.identity_card"/> and <see cref="ElementType.internal_passport"/>. <para/>The file can be decrypted and verified using the accompanying <see cref="EncryptedCredentials"/>.
        /// </summary>
        public PassportFile front_side { get; set; }
        /// <summary>
        /// Optional. Encrypted file with the reverse side of the document, provided by the user. <para/>Available for <see cref="ElementType.driver_license"/> and <see cref="ElementType.identity_card"/>. <para/>The file can be decrypted and verified using the accompanying <see cref="EncryptedCredentials"/>.
        /// </summary>
        public PassportFile reverse_side { get; set; }
        /// <summary>
        /// Optional. Encrypted file with the selfie of the user holding a document, provided by the user; <para/>available for <see cref="ElementType.passport"/>,<see cref="ElementType.driver_license"/>,<see cref="ElementType.identity_card"/>and <see cref="ElementType.internal_passport"/>. <para/>The file can be decrypted and verified using the accompanying <see cref="EncryptedCredentials"/>.
        /// </summary>
        public PassportFile selfie { get; set; }
        /// <summary>
        /// Optional. Array of encrypted files with translated versions of documents provided by the user. <para/>Available if requested for <see cref="ElementType.passport"/>, <see cref="ElementType.driver_license"/>, <see cref="ElementType.identity_card"/>, <see cref="ElementType.internal_passport"/>, <see cref="ElementType.utility_bill"/>, <see cref="ElementType.bank_statement"/>, <see cref="ElementType.rental_agreement"/>, <see cref="ElementType.passport_registration"/> and <see cref="ElementType.temporary_registration"/>. <para/>Files can be decrypted and verified using the accompanying <see cref="EncryptedCredentials"/>.
        /// </summary>
        public List<PassportFile> translation { get; set; }
        /// <summary>
        /// Base64-encoded element hash for using in PassportElementErrorUnspecified
        /// </summary>
        public string hash { get; set; }
    }

    public enum ElementType
    {
        personal_details,
        passport,
        driver_license,
        identity_card,
        internal_passport,
        address,
        utility_bill,
        bank_statement,
        rental_agreement,
        passport_registration,
        temporary_registration,
        phone_number,
        email
    }
}
