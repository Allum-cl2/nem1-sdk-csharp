﻿// ***********************************************************************
// Assembly         : nem2-sdk
// Author           : kailin
// Created          : 06-01-2018
//
// Last Modified By : kailin
// Last Modified On : 11-07-2018
// ***********************************************************************
// <copyright file="KeyPair.cs" company="Nem.io">   
// Copyright 2018 NEM
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
// http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using io.nem1.sdk.Core.Crypto.Chaso.NaCl;

namespace io.nem1.sdk.Model.Accounts
{
    /// <summary>
    /// The KeyPair structure describes a private key and public key in two formats, and contains static classes from creating the structure from raw data.
    /// </summary>
    /// <seealso cref="IKeyPair" />
    public class KeyPair : IKeyPair
    {

        /// <inheritdoc />
        public byte[] PrivateKey { get; }

        /// <inheritdoc />
        public byte[] PublicKey { get; }

        /// <summary>
        /// Gets the private key string.
        /// </summary>
        /// <value>The private key string.</value>
        public string PrivateKeyString => PrivateKey.ToHexLower().ToUpper();

        /// <summary>
        /// Gets the public key string.
        /// </summary>
        /// <value>The public key string.</value>
        public string PublicKeyString => PublicKey.ToHexLower().ToUpper();

        /// <summary>
        /// Initializes a new instance of the <see cref="KeyPair"/> class.
        /// </summary>
        /// <param name="privateKey">The private key.</param>
        /// <param name="publicKey">The public key.</param>
        /// <exception cref="ArgumentNullException">
        /// privateKey
        /// or
        /// publicKey
        /// </exception>
        /// <exception cref="ArgumentException">
        /// privateKey
        /// or
        /// publicKey
        /// </exception>
        internal KeyPair(string privateKey, string publicKey)
        {
            if (publicKey == null) throw new ArgumentNullException(nameof(publicKey));
            if (publicKey.Length != 64) throw new ArgumentException(nameof(publicKey));

            PrivateKey = privateKey.FromHex();

            PublicKey = publicKey.FromHex();
        }

        /// <summary>
        /// Creates a KeyPair from a private key.
        /// </summary>
        /// <param name="privateKey">The private key.</param>
        /// <returns>KeyPair.</returns>
        /// <exception cref="ArgumentNullException">privateKey</exception>
        /// <exception cref="ArgumentException">privateKey</exception>
        public static KeyPair CreateFromPrivateKey(string privateKey)
        {
            if (privateKey == null) throw new ArgumentNullException(nameof(privateKey));
            if (privateKey.Length != 64) throw new ArgumentException(nameof(privateKey));

            var privateKeyArray = privateKey.FromHex();
            Array.Reverse(privateKeyArray);
            return new KeyPair(privateKey, Ed25519.PublicKeyFromSeed(privateKeyArray).ToHexLower());
        }

        /// <summary>
        /// Signs the specified data with this keypair.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns>System.Byte[].</returns>
        /// <exception cref="ArgumentNullException">data</exception>
        public byte[] Sign(byte[] data)
        {
            if (data == null) throw new ArgumentNullException(nameof(data));
            var sig = new byte[64];
            var sk = new byte[64];

            Array.Copy(PrivateKey, sk, 32);

            Array.Copy(PublicKey, 0, sk, 32, 32);

            Ed25519.crypto_sign2(sig, data, sk, 32);

            CryptoBytes.Wipe(sk);

            return sig;
        }      
    }
}
