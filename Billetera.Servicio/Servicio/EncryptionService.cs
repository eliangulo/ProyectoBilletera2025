using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Billetera.BD.Datos;
using Billetera.BD.Datos.Entity;
using Microsoft.EntityFrameworkCore;

namespace Billetera.Servicios
{   //Encriptacion y Desencriptacion AES para ADMIN
    public interface IEncryptionService
    {
        string Encriptar(string textoPlano);
        string Desencriptar(string textoEncriptado);
        Task CrearAdminSiNoExiste(AppDbContext context);
    }

    public class EncryptionService : IEncryptionService
    {
        private readonly byte[] _key;
        private readonly byte[] _iv;

        public EncryptionService()
        {
            //  tus claves
            _key = Encoding.UTF8.GetBytes("MiClaveSecreta2025Administrador!");
            _iv = Encoding.UTF8.GetBytes("MiclaveAdmin2025");
        }

        public string Encriptar(string textoPlano)
        {
            if (string.IsNullOrEmpty(textoPlano))
                throw new ArgumentException("El texto no puede estar vacío");

            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = _key;
                aesAlg.IV = _iv;
                aesAlg.Mode = CipherMode.CBC;
                aesAlg.Padding = PaddingMode.PKCS7;

                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(textoPlano);
                        }
                    }
                    return Convert.ToBase64String(msEncrypt.ToArray());
                }
            }
        }

        public string Desencriptar(string textoEncriptado)
        {
            if (string.IsNullOrEmpty(textoEncriptado))
                throw new ArgumentException("El texto encriptado no puede estar vacío");

            byte[] buffer = Convert.FromBase64String(textoEncriptado);

            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = _key;
                aesAlg.IV = _iv;
                aesAlg.Mode = CipherMode.CBC;
                aesAlg.Padding = PaddingMode.PKCS7;

                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msDecrypt = new MemoryStream(buffer))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            return srDecrypt.ReadToEnd();
                        }
                    }
                }
            }
        }
        public async Task CrearAdminSiNoExiste(AppDbContext context)
        {
            // Verificar si ya existe un admin
            if (!await context.Usuarios.AnyAsync(u => u.EsAdmin == true))
            {
                // Crear billetera admin
                var billeteraAdmin = new Billeteras
                {
                    FechaCreacion = DateTime.Now,
                    Billera_Admin = true
                };
                context.Billeteras.Add(billeteraAdmin);
                await context.SaveChangesAsync();

                string claveAdmin = "MiclaveAdmin2025";

                var admin = new Usuarios
                {
                    BilleteraId = billeteraAdmin.Id,
                    CUIL = "20-12345678-9",
                    Nombre = "Administrador",
                    Apellido = "Sistema",
                    Domicilio = "Oficina Central de Pepe Sociedad Anonima",
                    FechaNacimiento = new DateTime(1960, 5, 1),
                    Correo = "admin@pepe_sa.com",
                    Telefono = "3512345678",
                    PasswordHash = Encriptar(claveAdmin),
                    EsAdmin = true
                };

                context.Usuarios.Add(admin);
                await context.SaveChangesAsync();

                Console.WriteLine("✅ Admin creado exitosamente");
                Console.WriteLine($"   Password para compartir: {claveAdmin}");
            }
        }
    }   
}
