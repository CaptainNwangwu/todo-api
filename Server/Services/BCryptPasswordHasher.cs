/* The provided code is a C# class named `PasswordHashingService` within the `Server.Services`
namespace. This class is responsible for hashing and verifying passwords using the BCrypt hashing
algorithm. */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BCrypt.Net;

namespace Server.Services
{
    public class BCryptPasswordHasher(IConfiguration config) : IPasswordHasher
    {
        private readonly IConfiguration _config = config;

        /// <summary>
        /// The HashPassword function hashes a given password using BCrypt with a specified work factor.
        /// </summary>
        /// <param name="password">The `HashPassword` method takes in a `password` as a parameter, which is the
        /// plain text password that you want to hash for security purposes. The method then uses the BCrypt
        /// hashing algorithm to securely hash the password before returning the hashed value.</param>
        /// <returns>
        /// The HashPassword method is returning the hashed version of the input password using the BCrypt
        /// hashing algorithm with the specified work factor.
        /// </returns>
        public string HashPassword(string password)
        {
            int workFactor = int.Parse(_config["SecuritySettings:HashWorkFactor"] ?? throw new InvalidOperationException("SecuritySettings:HashWorkFactor is not configured"));
            return BCrypt.Net.BCrypt.HashPassword(password, workFactor);
        }

        /// <summary>
        /// The VerifyPassword function in C# uses BCrypt.Net to verify a password against its hashed version.
        /// </summary>
        /// <param name="password">The `password` parameter is the plain text password that a user enters when
        /// trying to log in.</param>
        /// <param name="hashedPassword">The `hashedPassword` parameter in the `VerifyPassword` method is the
        /// hashed version of the original password. When a password is hashed, it is transformed into a
        /// fixed-length string of characters that cannot be reversed to obtain the original password. The
        /// purpose of hashing passwords is to securely store and compare them</param>
        /// <returns>
        /// The method `VerifyPassword` is returning a boolean value, which indicates whether the provided
        /// `password` matches the `hashedPassword` after verification using BCrypt hashing algorithm.
        /// </returns>
        public bool VerifyPassword(string password, string hashedPassword)
        {
            return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
        }
    }
}