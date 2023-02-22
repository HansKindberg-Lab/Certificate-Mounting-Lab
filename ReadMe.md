# Certificate-Mounting-Lab

Application for laborating with certificate-mounting in Linux containers.

Try different mounts by uncommenting lines in the [project-file](/Source/Application/Application.csproj#L6).

## [Certificates](/Certificates)

- **ca-certificate-bundle.crt:** is the ca-certificates.crt copied from the container running it without mounts (default).
- **root-certificate.crt**
- **intermediate-certificate-1.crt**
- **intermediate-certificate-2.crt**

The certificates in this solution are created by using this web-application, [Certificate-Factory](https://github.com/HansKindberg-Lab/Certificate-Factory). It is a web-application you can run in Visual Studio and then upload a json-certificate-file.

The json-certificate-file used for this solution:

	{
		"Defaults": {
			"HashAlgorithm": "Sha256",
			"NotAfter": "2050-01-01"
		},
		"Roots": {
			"root-certificate": {
				"Certificate": {
					"Subject": "CN=Test Root CA"
				},
				"IssuedCertificates": {
					"intermediate-certificate-1": {
						"Certificate": {
							"CertificateAuthority": {
								"PathLengthConstraint": 0
							},
							"KeyUsage": "KeyCertSign",
							"Subject": "CN=Test Intermediate 1"
						}
					},
					"intermediate-certificate-2": {
						"Certificate": {
							"CertificateAuthority": {
								"PathLengthConstraint": 0
							},
							"KeyUsage": "KeyCertSign",
							"Subject": "CN=Test Intermediate 2"
						}
					}
				}
			}
		},
		"RootsDefaults": {
			"CertificateAuthority": {
				"PathLengthConstraint": null
			},
			"KeyUsage": "KeyCertSign"
		}
	}

## Notes

### OpenShift / Kubernetes

In OpenShift/Kubernetes we can use configmaps to mount certificates.

- [Add SSL / TLS Certificate or .PEM file to Kubernetes’ Pod’s trusted root ca store](https://paraspatidar.medium.com/add-ssl-tls-certificate-or-pem-file-to-kubernetes-pod-s-trusted-root-ca-store-7bed5cd683d)

The thing is, when I try this in OpenShift (adding additional certificates), I can not get it to work. If this has something to do with "not running pod as root in OpenShift", I dont know. What works in OpenShift, for me, is by mounting a configmap with certificates at "/etc/ssl/certs", which wipes out all previous certificates in that directory. But that is the only way I have found is working. Maybe a solution with **initContainers** running the command "update-ca-certificates" will work, I dont know.

### update-ca-certificates

In this application we mount certificates / certificate-directory and we do not need to run **update-ca-certificates**.

- https://manpages.debian.org/buster/ca-certificates/update-ca-certificates.8.en.html