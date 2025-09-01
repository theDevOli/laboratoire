
DROP SCHEMA IF EXISTS cash_flow CASCADE;
CREATE SCHEMA cash_flow;

DROP SCHEMA IF EXISTS users CASCADE;
CREATE SCHEMA users;

DROP SCHEMA IF EXISTS document CASCADE;
CREATE SCHEMA document;

DROP SCHEMA IF EXISTS inventory CASCADE;
CREATE SCHEMA inventory;

DROP SCHEMA IF EXISTS parameters CASCADE;
CREATE SCHEMA parameters;

DROP SCHEMA IF EXISTS customers CASCADE;
CREATE SCHEMA customers;

DROP SCHEMA IF EXISTS utils CASCADE;
CREATE SCHEMA utils;

DROP SCHEMA IF EXISTS employee CASCADE;
CREATE SCHEMA employee;

DROP SEQUENCE IF EXISTS parameters.catalog_id_seq  CASCADE;
CREATE SEQUENCE parameters.catalog_id_seq START 1;

DROP TABLE IF EXISTS parameters.catalog;
CREATE TABLE parameters.catalog(
    CatalogId          INT PRIMARY KEY DEFAULT nextval('parameters.catalog_id_seq'),
    ReportType         VARCHAR(100) NOT NULL,
    SampleType         VARCHAR(100) NOT NULL,
    LabelName          VARCHAR(100) NOT NULL,
    Legends            JSONB[] NOT NULL,
    Price              DECIMAL(10,2) NOT NULL
);

DROP TABLE IF EXISTS employee.employee;
CREATE TABLE employee.employee(
    employee_id         UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    name                VARCHAR(100) NOT NULL,
    user_id             UUID REFERENCES users.user(user_id) NOT NULL
);

DROP SEQUENCE IF EXISTS parameters.parameter_id_seq  CASCADE;
CREATE SEQUENCE parameters.parameter_id_seq START 1;

DROP TABLE IF EXISTS parameters.parameter;
CREATE TABLE parameters.parameter(
    ParameterId         INT PRIMARY KEY DEFAULT nextval('parameters.parameter_id_seq'),
    CatalogId           INT REFERENCES parameters.catalog(CatalogId) NOT NULL,
    ParameterName       VARCHAR(80) NOT NULL,
    InputQuantity       INT DEFAULT 1,            
    Unit                VARCHAR(80),
    OfficialDoc         VARCHAR(80),
    Vmp                 VARCHAR(80),
    Equation            VARCHAR(80)
);

DROP TABLE IF EXISTS customers.client;
CREATE TABLE customers.client(
    ClientId           UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    ClientName         VARCHAR(100) NOT NULL,
    ClientTaxId        VARCHAR(14) NOT NULL,
    ClientEmail        VARCHAR(50),
    ClientPhone        VARCHAR(12)
);

DROP SEQUENCE IF EXISTS utils.state_id_seq CASCADE;
CREATE SEQUENCE utils.state_id_seq START 1;

DROP TABLE IF EXISTS utils.state;
CREATE TABLE utils.state(
    StateId         INT PRIMARY KEY DEFAULT nextval('utils.state_id_seq'),
    StateName       VARCHAR(50) NOT NULL,
    StateCode       VARCHAR(2) NOT NULL
);

DROP SEQUENCE IF EXISTS customers.property_id_seq CASCADE;
CREATE SEQUENCE customers.property_id_seq START 1;

DROP TABLE IF EXISTS customers.property;
CREATE TABLE customers.property(
    PropertyId         INT PRIMARY KEY DEFAULT nextval('customers.property_id_seq'),
    ClientId           UUID REFERENCES customers.client(ClientId) NOT NULL,
    StateId            INT REFERENCES utils.state(StateId) NOT NULL,
    PropertyName       VARCHAR(100) NOT NULL,
    City               VARCHAR(100) NOT NULL,
    -- Registration       VARCHAR(10),
    Registration       VARCHAR(100),
    PostalCode         VARCHAR(8),
    -- Area               VARCHAR(14),
    Area               VARCHAR(100),
    Ccir               VARCHAR(50),
    ItrNirf            VARCHAR(9)
);

DROP TABLE IF EXISTS customers.partner;
CREATE TABLE customers.partner(
    PartnerId           UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    PartnerName         VARCHAR(100) NOT NULL,
    OfficeName          VARCHAR(100) NOT NULL,
    PartnerPhone        VARCHAR(12) NOT NULL,
    PartnerEmail        VARCHAR(50) NOT NULL
);

DROP SEQUENCE IF EXISTS cash_flow.transaction_id_seq CASCADE;
CREATE SEQUENCE cash_flow.transaction_id_seq START 1;

DROP TABLE IF EXISTS cash_flow.transaction;
CREATE TABLE cash_flow.transaction(
    TransactionId       INT PRIMARY KEY DEFAULT nextval('cash_flow.transaction_id_seq'),
    TransactionType     VARCHAR(100) NOT NULL,
    OwnerName           VARCHAR(100),
    BankName            VARCHAR(100)
);

DROP SEQUENCE IF EXISTS cash_flow.cash_flow_id_seq CASCADE;
CREATE SEQUENCE cash_flow.cash_flow_id_seq START 1;

-- FIXME: To income is ok, but not for outcome, I need to remove CatalogId,ClientId,PropertyId and
-- create a new nullable atribute Description for outcome
DROP TABLE IF EXISTS cash_flow.cash_flow;
CREATE TABLE cash_flow.cash_flow(
    CashFlowId          INT PRIMARY KEY DEFAULT nextval('cash_flow.cash_flow_id_seq'),
    TransactionId       INT REFERENCES cash_flow.transaction(TransactionId) NOT NULL,
    Description         VARCHAR(100),
    PartnerId           UUID REFERENCES customers.partner(PartnerId),
    TotalPaid           DECIMAL(10,2),
    PaymentDate         DATE
);

DROP SEQUENCE IF EXISTS inventory.hazard_id_seq CASCADE;
CREATE SEQUENCE inventory.hazard_id_seq START 1;

DROP TABLE IF EXISTS inventory.hazard;
CREATE TABLE inventory.hazard(
    HazardId              INT PRIMARY KEY DEFAULT nextval('inventory.hazard_id_seq'),
    HazardClass           VARCHAR(5) NOT NULL,
    HazardName            VARCHAR(100) NOT NULL
);

DROP SEQUENCE IF EXISTS inventory.chemical_id_seq CASCADE;
CREATE SEQUENCE inventory.chemical_id_seq START 1;

DROP TABLE IF EXISTS inventory.chemical;
CREATE TABLE inventory.chemical(
    ChemicalId          INT PRIMARY KEY DEFAULT nextval('inventory.chemical_id_seq'),
    ChemicalName        VARCHAR(50) NOT NULL,
    Concentration       VARCHAR(50) NOT NULL,
    Quantity            DECIMAL(10,2) NOT NULL,
    Unit                VARCHAR(15) NOT NULL,
    PoliceControlled    BOOLEAN NOT NULL,
    ArmyControlled      BOOLEAN NOT NULL,
    EntryDate           DATE NOT NULL,
    ExpireDate          DATE NOT NULL
);

DROP TABLE IF EXISTS inventory.chemical_hazard;
CREATE TABLE inventory.chemical_hazard(
    ChemicalId          INT REFERENCES inventory.chemical(ChemicalId) NOT NULL,
    HazardId            INT REFERENCES inventory.hazard(HazardId) NOT NULL,
    PRIMARY KEY (ChemicalId,HazardId)
);


DROP SEQUENCE IF EXISTS inventory.solution_id_seq CASCADE;
CREATE SEQUENCE inventory.solution_id_seq START 1;

DROP TABLE IF EXISTS inventory.solution;
CREATE TABLE inventory.solution(
    SolutionId          INT PRIMARY KEY DEFAULT nextval('inventory.solution_id_seq'),
    ChemicalId          INT REFERENCES inventory.chemical(ChemicalId) NOT NULL,
    Concentration       VARCHAR(50) NOT NULL,
    ConcentrationUnit   VARCHAR(10) DEFAULT 'mL',
    Quantity            DECIMAL(10,2) NOT NULL,
    QuantityUnit        VARCHAR(10) DEFAULT 'mL',
    EntryDate           DATE NOT NULL,
    ExpireDate          DATE NOT NULL
);

DROP SEQUENCE IF EXISTS users.role_id_seq CASCADE;
CREATE SEQUENCE users.role_id_seq START 1;

DROP TABLE IF EXISTS users.role;
CREATE TABLE users.role(
    RoleId         INT PRIMARY KEY DEFAULT nextval('users.role_id_seq'),
    RoleName       VARCHAR(25) NOT NULL
);

DROP TABLE IF EXISTS users.user;
CREATE TABLE users.user(
    UserId          UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    RoleId          INT REFERENCES users.role(RoleId) NOT NULL,
    PartnerId       UUID REFERENCES customers.partner(PartnerId),
    -- Username        VARCHAR(25) NOT NULL,
    -- Name            VARCHAR(25) NOT NULL,
    Username        VARCHAR(100) NOT NULL,
    Name            VARCHAR(100) NOT NULL,
    IsActive        BOOLEAN DEFAULT true
);

DROP SEQUENCE IF EXISTS users.permission_seq CASCADE;
CREATE SEQUENCE users.permission_seq START 1;

DROP TABLE IF EXISTS users.permission;
CREATE TABLE users.permission(
    PermissionId    INT PRIMARY KEY DEFAULT nextval('users.permission_seq'),
    RoleId          INT REFERENCES users.role(RoleId) NOT NULL,
    Protocol        BOOLEAN DEFAULT FALSE,
    Client          BOOLEAN DEFAULT FALSE,
    Property        BOOLEAN DEFAULT FALSE,
    CashFlow        BOOLEAN DEFAULT FALSE,
    Partner         BOOLEAN DEFAULT FALSE,
    Users           BOOLEAN DEFAULT FALSE,
    Chemical        BOOLEAN DEFAULT FALSE
);

DROP TABLE IF EXISTS users.auth;
CREATE TABLE users.auth(
    UserId          UUID REFERENCES users.user(UserId) NOT NULL,
    PasswordSalt    BYTEA,
    PasswordHash    BYTEA
);

DROP SEQUENCE IF EXISTS document.crop_id_seq CASCADE;
CREATE SEQUENCE document.crop_id_seq START 1;

/*
    JSONB structure:
        {'min':15,'med':30,'max':45}
*/

DROP TABLE IF EXISTS document.crop;
CREATE TABLE document.crop(
    CropId              INT PRIMARY KEY DEFAULT nextval('document.crop_id_seq'),
    CropName            VARCHAR(50) NOT NULL,
    NitrogenCover       INT,
    NitrogenFoundation  INT NOT NULL,
    Phosphorus          JSONB NOT NULL,
    Potassium           JSONB NOT NULL,
    MinV                INT NOT NULL,
    ExtraData           VARCHAR(150)
);

DROP SEQUENCE IF EXISTS document.fertilizer_id_seq CASCADE;
CREATE SEQUENCE document.fertilizer_id_seq START 1;

/*
    JSONB structure:
        {'min':15,'med':30,'max':45}
*/

DROP TABLE IF EXISTS document.fertilizer;
CREATE TABLE document.fertilizer(
    FertilizerId        INT PRIMARY KEY DEFAULT nextval('document.fertilizer_id_seq'),
    Nitrogen            INT NOT NULL,
    Phosphorus          INT NOT NULL,
    Potassium           INT NOT NULL,
    IsAvailable         BOOLEAN NOT NULL,
    Proportion          VARCHAR(15)
);

CREATE OR REPLACE FUNCTION document.set_fertilizer_proportion()
RETURNS TRIGGER AS $$
DECLARE
    min_value INT;
    n INT;
    p INT;
    k INT;
BEGIN
    -- Captura os valores de Nitrogen, Phosphorus e Potassium
    n := NEW.Nitrogen;
    p := NEW.Phosphorus;
    k := NEW.Potassium;

    -- Determina o menor valor
    min_value := LEAST(n, p, k);

    -- Divide todos os valores pelo menor
    NEW.Proportion := 
        (n / min_value) || '-' || 
        (p / min_value) || '-' || 
        (k / min_value);

    -- Retorna o novo registro com a Proportion calculada
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

CREATE TRIGGER trigger_set_fertilizer_proportion
BEFORE INSERT ON document.fertilizer
FOR EACH ROW
EXECUTE FUNCTION document.set_fertilizer_proportion();

DROP TABLE IF EXISTS document.report;
CREATE TABLE document.report(
    ReportId            UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    ProtocolId          VARCHAR(9) NOT NULL,
    Results             JSONB[]
);

DROP TABLE IF EXISTS document.protocol;
CREATE TABLE document.protocol(
    ProtocolId          VARCHAR(9),
    CashFlowId          INT REFERENCES cash_flow.cash_flow(CashFlowId),
    ReportId            UUID REFERENCES document.report(ReportId),
    ClientId            UUID REFERENCES customers.client(ClientId) NOT NULL,
    PropertyId          INT REFERENCES customers.property(PropertyId) NOT NULL,
    PartnerId           UUID REFERENCES customers.partner(PartnerId),
    CatalogId           INT REFERENCES parameters.catalog(CatalogId) NOT NULL,
    EntryDate           DATE NOT NULL,
    ReportDate          DATE,
    IsCollectedByClient BOOLEAN NOT NULL,
    PRIMARY KEY (ProtocolId, EntryDate)
)PARTITION BY RANGE (EntryDate);

DROP TABLE IF EXISTS document.crop_protocol;
CREATE TABLE document.crop_protocol(
    CropId              INT REFERENCES document.crop(CropId) NOT NULL,
    ProtocolId          VARCHAR(9) NOT NULL,
    PRIMARY KEY (CropId, ProtocolId)
);

CREATE OR REPLACE FUNCTION document.set_protocol_id()
RETURNS TRIGGER AS $$
DECLARE
    row_count INT;
    entry_year INT;
BEGIN
    -- Pegar o ano do EntryDate que está sendo inserido
    entry_year := EXTRACT(YEAR FROM NEW.entry_date);

    -- Contar os registros do mesmo ano do EntryDate do novo documento
    row_count := (
        SELECT COUNT(*)
        FROM document.protocol
        WHERE EXTRACT(YEAR FROM entry_date) = entry_year
    ) + 1;

    -- Formatar o ProtocolId no estilo '0004/2024'
    NEW.protocol_id := LPAD(row_count::TEXT, 4, '0') || '/' || entry_year::TEXT;

    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

CREATE TRIGGER before_insert_protocol_id
BEFORE INSERT ON document.protocol
FOR EACH ROW
EXECUTE FUNCTION document.set_protocol_id();

--Handle the partition
-- Partition for entries in 2011
CREATE TABLE document.protocol_2011 
PARTITION OF document.protocol 
FOR VALUES FROM ('2011-01-01') TO ('2012-01-01');

-- Partition for entries in 2012
CREATE TABLE document.protocol_2012 
PARTITION OF document.protocol 
FOR VALUES FROM ('2012-01-01') TO ('2013-01-01');

-- Partition for entries in 2013
CREATE TABLE document.protocol_2013 
PARTITION OF document.protocol 
FOR VALUES FROM ('2013-01-01') TO ('2014-01-01');

-- Partition for entries in 2014
CREATE TABLE document.protocol_2014 
PARTITION OF document.protocol 
FOR VALUES FROM ('2014-01-01') TO ('2015-01-01');

-- Partition for entries in 2015
CREATE TABLE document.protocol_2015 
PARTITION OF document.protocol 
FOR VALUES FROM ('2015-01-01') TO ('2016-01-01');

-- Partition for entries in 2016
CREATE TABLE document.protocol_2016 
PARTITION OF document.protocol 
FOR VALUES FROM ('2016-01-01') TO ('2017-01-01');

-- Partition for entries in 2017
CREATE TABLE document.protocol_2017 
PARTITION OF document.protocol 
FOR VALUES FROM ('2017-01-01') TO ('2018-01-01');

-- Partition for entries in 2018
CREATE TABLE document.protocol_2018 
PARTITION OF document.protocol 
FOR VALUES FROM ('2018-01-01') TO ('2019-01-01');

-- Partition for entries in 2019
CREATE TABLE document.protocol_2019 
PARTITION OF document.protocol 
FOR VALUES FROM ('2019-01-01') TO ('2020-01-01');

-- Partition for entries in 2020
CREATE TABLE document.protocol_2020 
PARTITION OF document.protocol 
FOR VALUES FROM ('2020-01-01') TO ('2021-01-01');

-- Partition for entries in 2021
CREATE TABLE document.protocol_2021
PARTITION OF document.protocol 
FOR VALUES FROM ('2021-01-01') TO ('2022-01-01');

-- Partition for entries in 2022
CREATE TABLE document.protocol_2022
PARTITION OF document.protocol 
FOR VALUES FROM ('2022-01-01') TO ('2023-01-01');

-- Partition for entries in 2023
CREATE TABLE document.protocol_2023
PARTITION OF document.protocol 
FOR VALUES FROM ('2023-01-01') TO ('2024-01-01');

-- Partition for entries in 2024
CREATE TABLE document.protocol_2024
PARTITION OF document.protocol 
FOR VALUES FROM ('2024-01-01') TO ('2025-01-01');

-- Partition for entries in 2025
CREATE TABLE document.protocol_2025
PARTITION OF document.protocol
FOR VALUES FROM ('2025-01-01') TO ('2026-01-01');

-- Partition for entries in 2026
CREATE TABLE document.protocol_2026
PARTITION OF document.protocol
FOR VALUES FROM ('2026-01-01') TO ('2027-01-01');

-- Partition for entries in 2027
CREATE TABLE document.protocol_2027
PARTITION OF document.protocol
FOR VALUES FROM ('2027-01-01') TO ('2028-01-01');

-- Partition for entries in 2028
CREATE TABLE document.protocol_2028
PARTITION OF document.protocol
FOR VALUES FROM ('2028-01-01') TO ('2029-01-01');

-- Partition for entries in 2029
CREATE TABLE document.protocol_2029
PARTITION OF document.protocol
FOR VALUES FROM ('2029-01-01') TO ('2030-01-01');

-- Partition for entries in 2030
CREATE TABLE document.protocol_2030
PARTITION OF document.protocol
FOR VALUES FROM ('2030-01-01') TO ('2031-01-01');

-- Default partition
CREATE TABLE document.protocol_default
PARTITION OF document.protocol DEFAULT;

CREATE INDEX idx_protocol_2011_entry_date ON document.protocol_2011 (EntryDate);
CREATE INDEX idx_protocol_2012_entry_date ON document.protocol_2012 (EntryDate);
CREATE INDEX idx_protocol_2013_entry_date ON document.protocol_2013 (EntryDate);
CREATE INDEX idx_protocol_2014_entry_date ON document.protocol_2014 (EntryDate);
CREATE INDEX idx_protocol_2015_entry_date ON document.protocol_2015 (EntryDate);
CREATE INDEX idx_protocol_2016_entry_date ON document.protocol_2016 (EntryDate);
CREATE INDEX idx_protocol_2017_entry_date ON document.protocol_2017 (EntryDate);
CREATE INDEX idx_protocol_2018_entry_date ON document.protocol_2018 (EntryDate);
CREATE INDEX idx_protocol_2019_entry_date ON document.protocol_2019 (EntryDate);
CREATE INDEX idx_protocol_2020_entry_date ON document.protocol_2020 (EntryDate);
CREATE INDEX idx_protocol_2021_entry_date ON document.protocol_2021 (EntryDate);
CREATE INDEX idx_protocol_2022_entry_date ON document.protocol_2022 (EntryDate);
CREATE INDEX idx_protocol_2023_entry_date ON document.protocol_2023 (EntryDate);
CREATE INDEX idx_protocol_2024_entry_date ON document.protocol_2024 (EntryDate);
CREATE INDEX idx_protocol_2025_entry_date ON document.protocol_2025 (EntryDate);
CREATE INDEX idx_protocol_2026_entry_date ON document.protocol_2026 (EntryDate);
CREATE INDEX idx_protocol_2027_entry_date ON document.protocol_2027 (EntryDate);
CREATE INDEX idx_protocol_2028_entry_date ON document.protocol_2028 (EntryDate);
CREATE INDEX idx_protocol_2029_entry_date ON document.protocol_2029 (EntryDate);
CREATE INDEX idx_protocol_2030_entry_date ON document.protocol_2030 (EntryDate);