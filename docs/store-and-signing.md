# Microsoft Store listing and code-signing notes

Nothing here has been submitted. These notes prepare the owner to publish.

## Code signing (not available yet)
- Releases are **unsigned**. Expect SmartScreen "Windows protected your PC" until files are signed and build reputation.
- Azure Artifact Signing (formerly Trusted Signing, ~US$9.99/month) is limited to organisations in specific regions and individuals in the USA/Canada; Egypt is not listed. Realistic options: an OV code-signing certificate from a CA (roughly US$150–300/year, hardware token or cloud HSM), or the Microsoft Store (Microsoft re-signs MSIX packages).
- EV certificates no longer bypass SmartScreen (Microsoft change, 2024); not worth the premium for that reason.
- SignPath Foundation (free for OSS) excludes tools that scan for security weaknesses; Port Checker and Health Check would likely not qualify.
- To sign in CI later: add the certificate as a secret and run `signtool sign /fd SHA256 /tr http://timestamp.digicert.com /td SHA256` on each exe before hashing (hash after signing).

## Microsoft Store (needs the owner's account)
- Free individual registration at https://storedeveloper.microsoft.com (ID + selfie verification).
- Package as MSIX: add a Windows Application Packaging Project or use `dotnet publish` with `-p:WindowsPackageType=MSIX` via the Windows App SDK tooling, or `MakeAppx.exe pack` with an `AppxManifest.xml` declaring `runFullTrust` (WPF desktop apps are full-trust). Store re-signs the package.
- Capabilities to declare: `runFullTrust`; Wi-Fi Checker additionally `wiFiControl` is not needed for scanning via wlanapi but location consent applies on Windows 11 24H2+.
- Apps that change system settings or scan networks get closer certification review; start with Hash Checker, Password Checker, Header Analyzer, DNS Benchmark.

### Listing text (English)
**NetSec Atlas Hash Checker** — Verify that a downloaded file is exactly what the publisher released. Drag a file in, paste the expected SHA-256 (or load a SHA256SUMS file) and get a clear match / no-match. Supports MD5, SHA-1, SHA-256, SHA-384 and SHA-512, compares two files, and can add an optional right-click entry. Works offline; no telemetry. Arabic and English.

**NetSec Atlas Password Checker** — See how strong a password really is, offline. Optional breach check uses Have I Been Pwned's k-anonymity range API so your password never leaves your PC.

### نص المتجر (عربي)
**فاحص البصمة من NetSec Atlas** — تحقق أن الملف الذي نزّلته هو نفسه الذي نشره المطوّر. اسحب الملف والصق بصمة SHA-256 المتوقعة لتحصل على نتيجة واضحة: مطابق أو غير مطابق. يدعم MD5 وSHA-1 وSHA-256 وSHA-384 وSHA-512، يعمل دون إنترنت وبدون أي قياسات استخدام.
