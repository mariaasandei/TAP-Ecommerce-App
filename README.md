# 🛒 E-Commerce Order System (Console App)

O aplicație de consolă complexă dezvoltată în C# care simulează fluxul complet al unui magazin online: de la vizualizarea produselor până la procesarea plății și expedierea coletului.

---

## 👥 Utilizatorii Sistemului (End Users)

1.  **Cumpărătorul (Customer):** Navighează prin catalog, adaugă produse în coș și plasează comenzi.
2.  **Administratorul Magazinului (Shop Admin):** Gestionează campaniile de discount și catalogul de produse.
3.  **Operatorul Logistic (Logistic Manager):** Selectează curierii potriviți pentru livrare și monitorizează statusul comenzilor.

---

## ✨ Funcționalități Principale

* **Catalog de Produse:** Listare și filtrare produse.
* **Coș de Cumpărături:** Adăugare/Eliminare produse și calcul automat al totalului.
* **Sistem de Comenzi Versatil:** Suportă integrarea cu multiple sisteme de plată (ex: Card, PayPal, Crypto).
* **Motor de Discount-uri:** Arhitectură pregătită pentru campanii sezoniere sau coduri promoționale.
* **Expediere Multi-Courier:** Integrare cu diverse companii de curierat (ex: FAN Courier, Sameday, DHL).

---

## 🏗️ Arhitectură & Model C4

Proiectul respectă metodologia C4 pentru vizualizarea arhitecturii software:

### 1. System Context Diagram
Prezintă modul în care sistemul interacționează cu utilizatorii și serviciile externe (Sisteme de Plată, API-uri Curieri).



### 2. Container Diagram
Detaliază interiorul aplicației (Console App, File Database/Memory Storage, External API Wrappers).



---

## 🛠️ Design Patterns Utilizate

* **Strategy Pattern:** Folosit pentru `IPaymentSystem` și `ICourierService`, permițând schimbarea furnizorului fără a modifica logica de bază.
* **Repository Pattern:** Pentru gestionarea produselor și a comenzilor în memorie.
* **Decorator sau Strategy:** Pentru aplicarea flexibilă a discount-urilor.

---

## 💻 Instalare și Rulare

1. **Prerechizite:** .NET 8 SDK.
2. **Clonare:**
   ```bash
   git clone [https://github.com/utilizator/ecommerce-order-system.git](https://github.com/utilizator/ecommerce-order-system.git)
