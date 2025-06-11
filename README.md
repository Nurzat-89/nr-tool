# KazNRDC tool to calculate nuclide number densities for Massive Stars
This application is designed to calculate the nuclear number density of isotopes in massive stars as a function of time. It is built to support advanced research in nuclear astrophysics, particularly in modeling stellar nucleosynthesis. The application relies on ENDF (Evaluated Nuclear Data File) datasets for nuclear reaction data.

## Features
* **Nuclear Number Density Calculation:** Calculates isotope concentrations over time using input nuclear data.
* **High-Accuracy Methods:** Implements robust numerical methods like CRAM (Chebyshev Rational Approximation Method) and MMPA for matrix exponentials.
* **Customizable Input:** Supports user-provided ENDF files for tailored calculations

## Prerequisites
1. System Requirements
   * **Operating System:** Windows
   * **.NET Runtime:** .NET 6.0 or later
   * **RAM:** Minimum 4GB (higher recommended for large datasets)
2. Dependencies
   * **ENDF (Evaluated Nuclear Data Files) datasets.** You can obtain these from [IAEA ENDF Database](https://www-nds.iaea.org/exfor/endf.htm).

# Installation
* **Download the Data Library.** First download and install Evaluated Nuclear Data File (ENDF) installer:
  * [ENDFB-VIII](https://drive.google.com/file/d/13xvVk2kN6klo8WLAxsGl8bLR67xJtqiF/view?usp=sharing)
  * [JENDL](https://drive.google.com/file/d/1ZUGER21RiLy08cYjsrOBP2Pi6cUs3q39/view?usp=sharing)
  * [JEFF](https://drive.google.com/file/d/1ZUGER21RiLy08cYjsrOBP2Pi6cUs3q39/view?usp=sharing)
* **Download the Tool.** Download the latest version of the KazNRDC tool:
  *  [1.0.8](https://drive.google.com/file/d/1U0va1cqZu9QoY2ZNv_KP7G2VaaOuCR8F/view?usp=sharing)
  *  [1.0.7](https://drive.google.com/file/d/1PUJySQ6-ycmlVg-92R-sQ-nx_wHBlhtf/view?usp=sharing)
  *  [1.0.6](https://drive.google.com/file/d/1uJD0mueM_WGa90zDZfMG45u1vXzJgVTi/view?usp=sharing)
  *  [1.0.5](https://drive.google.com/file/d/1Z0W1F7b07-5T1ufLCoOc3Wirwj1-ez4u/view?usp=sharing)
* **Default Data Library.** The application currently includes only one default data library: ENDFB-VIII. The associated data files are located in:
  > ```%LocalAppData%/KazNRDC/xsdir```
* **Additional Data Libraries.**
  * It's also possible to download the latest version of data libraries from their official web pages:
    * [ENDF-B](https://www.nndc.bnl.gov/endf/?utm_source=chatgpt.com)
    * [JENDL](https://wwwndc.jaea.go.jp/jendl/j5/j5.html?utm_source=chatgpt.com)
    * [JEFF](https://www.oecd-nea.org/dbdata/jeff/jeff40/t3/?utm_source=chatgpt.com)
    * [TENDL](https://tendl.web.psi.ch/tendl_2023/tendl2023.html?utm_source=chatgpt.com)
* **MACS** *  
  * For nuclear astrophysics calculations, the MACS (Maxwellian-Averaged Cross Section) data library is also a valuable resource. MACS values represent neutron capture cross sections averaged over a Maxwell–Boltzmann distribution of neutron energies, typically at stellar temperatures (e.g. 30 keV).
    
| MACS Source | Description | URL |
|-------------|-------------|-----|
| **KADoNiS** | Karlsruhe Astrophysical Database of Nucleosynthesis in Stars (main MACS database for astrophysics) | [kadonis.org](https://www.kadonis.org) |
| **IAEA EXFOR MACS Calculator** | IAEA's online tool to compute MACS from evaluated data or experimental EXFOR entries | [macs_calc.html](https://www-nds.iaea.org/astro/macs_calc.html) |
| **IAEA EXFOR** | Experimental nuclear reaction database (raw data used to compute MACS) | [exfor](https://www-nds.iaea.org/exfor) |
| **TENDL (via TALYS)** | Theoretical nuclear data, from which MACS can be calculated using TALYS | [tendl.web.psi.ch](https://tendl.web.psi.ch) |
| **REACLIB** | Reaction rate library for astrophysical network calculations (rates can be converted to MACS) | [reaclib.jinaweb.org](https://reaclib.jinaweb.org) |
| **ENDF / JEFF / JENDL** | Evaluated nuclear data files used to compute MACS with processing tools | [ENDF](https://www.nndc.bnl.gov/endf/), [JEFF](https://www.oecd-nea.org/dbdata/jeff/), [JENDL](https://wwwndc.jaea.go.jp/jendl/) |

  * If you have some questions or need support, please contact: Nurzat Kenzhebaev 📧 nurzat.kenzhebaev@gmail.com
