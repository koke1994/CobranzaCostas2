param(
	[string]$KeystorePath = "keystore.jks",
	[string]$Alias = "mykey",
	[string]$StorePassword,
	[string]$KeyPassword,
	[string]$Name = "Cobranza Costas",
	[string]$Organization = "GCC",
	[string]$Country = "MX",
	[string]$Configuration = "Release",
	[string]$Output = ".\publish",
	[switch]$ForceCreate
)

function Convert-SecureStringToPlain([System.Security.SecureString]$ss) {
	$ptr = [Runtime.InteropServices.Marshal]::SecureStringToBSTR($ss)
	try { [Runtime.InteropServices.Marshal]::PtrToStringBSTR($ptr) }
	finally { [Runtime.InteropServices.Marshal]::ZeroFreeBSTR($ptr) }
}

# Pedir contraseñas si no se proporcionaron
if (-not $StorePassword) {
	$s = Read-Host -AsSecureString "Introduce la contraseña del keystore"
	$StorePassword = Convert-SecureStringToPlain $s
}
if (-not $KeyPassword) {
	$k = Read-Host -AsSecureString "Introduce la contraseña de la clave (puede ser la misma)"
	$KeyPassword = Convert-SecureStringToPlain $k
}

$keystoreFullPath = Resolve-Path -Path $KeystorePath -ErrorAction SilentlyContinue
if (-not $keystoreFullPath -or $ForceCreate) {
	Write-Host "Keystore no encontrado o se forzó creación. Creando $KeystorePath..."

	$keystoreDir = Split-Path -Path $KeystorePath -Parent
	if ($keystoreDir -and -not (Test-Path $keystoreDir)) { New-Item -ItemType Directory -Path $keystoreDir | Out-Null }

	$dname = "CN=$Name, OU=$Organization, O=$Organization, L=City, ST=State, C=$Country"

	$keytool = "keytool"
	$args = @("-genkeypair", "-v", "-keystore", $KeystorePath, "-storepass", $StorePassword, "-keypass", $KeyPassword, "-alias", $Alias, "-keyalg", "RSA", "-keysize", "2048", "-validity", "10000", "-dname", $dname)

	Write-Host "Ejecutando keytool para crear el keystore (asegúrate de tener Java JDK instalado)..."
	$proc = Start-Process -FilePath $keytool -ArgumentList $args -NoNewWindow -Wait -PassThru
n    if ($proc.ExitCode -ne 0) {
		Write-Error "Error creando keystore. Comprueba que keytool esté disponible y los parámetros.";
		exit $proc.ExitCode
	}
	else {
		Write-Host "Keystore creado: $KeystorePath"
	}
}
else {
	$keystoreFullPath = Resolve-Path -Path $KeystorePath
	$KeystorePath = $keystoreFullPath.Path
	Write-Host "Usando keystore existente: $KeystorePath"
}

# Publicar AAB firmado
Write-Host "Iniciando dotnet publish para generar AAB firmado..."
$publishArgs = @(
	'publish',
	'-f', 'net10.0-android',
	'-c', $Configuration,
	'-p:AndroidPackageFormat=aab',
	'-p:AndroidKeyStore=true',
	"-p:AndroidSigningKeyStore=$KeystorePath",
	"-p:AndroidSigningStorePass=$StorePassword",
	"-p:AndroidSigningKeyAlias=$Alias",
	"-p:AndroidSigningKeyPass=$KeyPassword",
	'-o', $Output
)

$dotnet = "dotnet"
$proc = Start-Process -FilePath $dotnet -ArgumentList $publishArgs -NoNewWindow -Wait -PassThru
if ($proc.ExitCode -ne 0) {
	Write-Error "dotnet publish falló con código $($proc.ExitCode). Revisa la salida para detalles.";
	exit $proc.ExitCode
}

Write-Host "Publicación completada. Bundle generado en: $Output"
Write-Host "Archivos en carpeta:"
Get-ChildItem -Path $Output -Filter *.aab -Recurse | ForEach-Object { Write-Host $_.FullName }
