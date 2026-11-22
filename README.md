# NetSec Atlas Apps

Free, open-source Windows tools from [NetSec Atlas](https://netsecatlas.com) for everyday users, network admins and security teams.
Bilingual (English / العربية with full right-to-left layout), single-file, no installer, no .NET install needed.


## Status (22 November 2026, Cairo time)

- **Code:** 11 WPF apps, shared core, unit tests, MIT, .NET 10 LTS. Last commit: `8337fb6` (19 Nov 2026, 12:40).
- **CI:** [run 44507937129](https://github.com/abdelaziz-maysara-hm/netsecatlas-apps/actions/runs/44507937129) succeeded the same day (tests + publish win-x64 and win-arm64, ~5m47s). The **release job was skipped** because it only runs on a `v*` tag, so there is **no GitHub Release and no public SHA-256 file yet**. Build artifacts (exes + screenshots) from that run remain downloadable from the Actions tab until about 17 February 2027.
- **Not done:** tagging `v0.1.0` (this creates the Release with `.exe` + `SHA256SUMS.txt`); code signing; Microsoft Store listing (needs the owner's Store account); and the netsecatlas.com `/programs` download pages (not shipped). See the site repo's [docs/ROADMAP.md](https://github.com/abdelaziz-maysara-hm/Tech-Insights/blob/main/docs/ROADMAP.md).
- **To publish a release:** `git tag v0.1.0 && git push origin v0.1.0` (or create the tag in GitHub). The workflow will attach the exes and checksums.

---

## الحالة (22 نوفمبر 2026، توقيت القاهرة)

- أحد عشر برنامجًا جاهزة في الشيفرة، والبناء والاختبارات نجحت في 19 نوفمبر.
- **لا يوجد إصدار GitHub بعد** لأن خطوة الإطلاق تنتظر وسمًا مثل `v0.1.0`. ملفات exe المؤقتة ما زالت في صفحة Actions حتى حوالي 17 فبراير 2027.
- لم يُنشَر شيء على Microsoft Store، والبرامج غير موقَّعة، وصفحات التحميل على الموقع غير مكتملة. التفاصيل في خارطة طريق الموقع.

**Privacy:** no telemetry, no ads, no accounts. Apps only touch the network when the feature itself needs it (for example a DNS benchmark or a TLS check), and the password checker sends only a 5-character hash prefix to the Have I Been Pwned range API.

| App | What it does | Network use |
|---|---|---|
| Hash Checker | MD5/SHA-1/SHA-256/SHA-384/SHA-512, compare to an expected hash or SHA256SUMS file, optional per-user Explorer menu | none |
| Security Health Check | Read-only check of antivirus, firewall, updates, UAC, BitLocker, SMBv1, RDP, Secure Boot, Windows version; score + report | none |
| Network Troubleshooter | Adapter → IP → gateway → DNS → ping → TCP port → route, with a plain-language diagnosis and report | to the target you enter |
| Password Checker | Offline strength estimate; optional breach check (k-anonymity) | api.pwnedpasswords.com (optional) |
| AD Lockout Tracer | Events 4740/4625/4771/4776 across domain controllers, ranks the likely source, CSV | your DCs |
| TLS Certificate Expiry Checker | Bulk days-left, name match, chain, protocol; CSV | the hosts you list |
| Email Header Analyzer | Received chain, delays, SPF/DKIM/DMARC, Reply-To and display-name tricks | none |
| Port & Service Checker | Listening ports with owning process, connections, remote port test, CSV | only for remote tests |
| Wi-Fi Checker | Nearby networks, channel, signal, WPA2/WPA3/WEP/open rating, 2.4 GHz channel advice | none (local scan) |
| DNS Benchmark | Cached/uncached latency of your DNS and well-known public resolvers | DNS queries to those resolvers |
| Ping Monitor | Continuous latency, jitter and loss for several hosts, live chart, CSV | ICMP to the hosts you list |

## Download

Get the `.exe` files from [Releases](https://github.com/abdelaziz-maysara-hm/netsecatlas-apps/releases/latest) or from [netsecatlas.com/en/programs](https://netsecatlas.com/en/programs).
Always compare the SHA-256 with `SHA256SUMS.txt`:

```powershell
Get-FileHash .\NetSecAtlas-HashChecker-win-x64.exe -Algorithm SHA256
```

> **Not code-signed yet.** Windows SmartScreen may say "Windows protected your PC". Check the hash, then choose *More info → Run anyway*.
> Single-file .NET apps unpack their native runtime files to a temp folder on first run; some antivirus products flag this pattern. Report false positives to your vendor.

Requirements: Windows 10/11 (x64 or ARM64). Command-line options for every app: `--lang ar|en`, `--demo`, `--screenshot file.png`.

## Build

```powershell
dotnet test tests/NetSecAtlas.Core.Tests
./build/publish.ps1 -Rid win-x64 -Out dist
```

Layout: `src/NetSecAtlas.Core` (cross-platform logic, unit tested), `src/NetSecAtlas.Ui` (shared WPF shell, bilingual/RTL), `apps/*` (one WPF app each). .NET 10 LTS.
CI (`.github/workflows/ci.yml`) builds and tests on `windows-latest`, publishes x64 + ARM64, captures real screenshots, and on a `v*` tag creates a release with `SHA256SUMS.txt`.

---

# تطبيقات NetSec Atlas

أدوات ويندوز مجانية ومفتوحة المصدر من [NetSec Atlas](https://netsecatlas.com) للمستخدم العادي ومسؤولي الشبكات وفرق الأمن. واجهة بالعربية والإنجليزية مع دعم كامل للاتجاه من اليمين لليسار، ملف واحد بدون تثبيت وبدون الحاجة لتثبيت .NET.

**الخصوصية:** لا قياسات استخدام ولا إعلانات ولا حسابات. لا تتصل البرامج بالشبكة إلا عندما تتطلب الميزة ذلك، وفاحص كلمات المرور يرسل فقط أول 5 أحرف من البصمة إلى خدمة Have I Been Pwned.

البرامج: فاحص البصمة، فحص صحة الأمان، مشخّص الشبكة، فاحص كلمات المرور، متتبع قفل حسابات AD، فاحص انتهاء شهادات TLS، محلل رؤوس البريد، فاحص المنافذ والخدمات، فاحص Wi-Fi، مقارنة خوادم DNS، مراقب Ping.

**التنزيل:** من صفحة [الإصدارات](https://github.com/abdelaziz-maysara-hm/netsecatlas-apps/releases/latest) أو من [netsecatlas.com/ar/programs](https://netsecatlas.com/ar/programs). قارن دائمًا بصمة SHA-256 بملف `SHA256SUMS.txt`.

> **البرامج غير موقّعة رقميًا حتى الآن**، لذلك قد يظهر تحذير SmartScreen «حمى Windows جهاز الكمبيوتر». تحقق من البصمة ثم اختر «مزيد من المعلومات ← التشغيل على أي حال».

الترخيص: MIT.
