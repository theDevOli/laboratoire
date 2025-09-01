
-- ALTER TABLE users.user
--     ALTER COLUMN Username TYPE VARCHAR(100),
--     ALTER COLUMN Name TYPE VARCHAR(100);

-- ALTER TABLE customers.property
--     ALTER COLUMN Registration TYPE VARCHAR(25);

-- ALTER TABLE customers.property
-- ALTER COLUMN Area TYPE VARCHAR(40),
-- ALTER COLUMN Registration TYPE VARCHAR(40);

--Alter the users.user
ALTER TABLE users.user DROP COLUMN name;
ALTER TABLE users.user RENAME COLUMN userid TO user_id;
ALTER TABLE users.user RENAME COLUMN roleid TO role_id;
ALTER TABLE users.user RENAME COLUMN partnerid TO partner_id;
ALTER TABLE users.user RENAME COLUMN isactive TO is_active;

--Alter users.auth
ALTER TABLE users.auth RENAME COLUMN userid TO user_id;
ALTER TABLE users.auth RENAME COLUMN passwordsalt TO password_salt;
ALTER TABLE users.auth RENAME COLUMN passwordhash TO password_hash;

--Alter cash_flow.cash_flow
ALTER TABLE cash_flow.cash_flow RENAME COLUMN cashflowid TO cash_flow_id;
ALTER TABLE cash_flow.cash_flow RENAME COLUMN transactionid TO transaction_id;
ALTER TABLE cash_flow.cash_flow RENAME COLUMN partnerid TO partner_id;
ALTER TABLE cash_flow.cash_flow RENAME COLUMN totalpaid TO total_paid;
ALTER TABLE cash_flow.cash_flow RENAME COLUMN paymentdate TO payment_date;

--Alter catalog
ALTER TABLE parameters.catalog RENAME COLUMN catalogid TO catalog_id;
ALTER TABLE parameters.catalog RENAME COLUMN reporttype TO report_type;
ALTER TABLE parameters.catalog RENAME COLUMN sampletype TO sample_type;
ALTER TABLE parameters.catalog RENAME COLUMN labelname TO label_name;

--Alter Chemical
ALTER TABLE inventory.chemical RENAME COLUMN chemicalid TO chemical_id;
ALTER TABLE inventory.chemical RENAME COLUMN chemicalname TO chemical_name;
ALTER TABLE inventory.chemical RENAME COLUMN policecontrolled TO is_police_controlled;
ALTER TABLE inventory.chemical RENAME COLUMN armycontrolled TO is_army_controlled;
ALTER TABLE inventory.chemical RENAME COLUMN entrydate TO entry_date;
ALTER TABLE inventory.chemical RENAME COLUMN expiredate TO expire_date;

--Alter for inventory.chemical_hazard
ALTER TABLE inventory.chemical_hazard RENAME COLUMN chemicalid TO chemical_id;
ALTER TABLE inventory.chemical_hazard RENAME COLUMN hazardid TO hazard_id;

--Alter for customers.client
ALTER TABLE customers.client RENAME COLUMN clientid TO client_id;
ALTER TABLE customers.client RENAME COLUMN clientname TO client_name;
ALTER TABLE customers.client RENAME COLUMN clienttaxid TO client_tax_id;
ALTER TABLE customers.client RENAME COLUMN clientemail TO client_email;
ALTER TABLE customers.client RENAME COLUMN clientphone TO client_phone;

--Alter for document.crop
ALTER TABLE document.crop RENAME COLUMN cropid TO crop_id;
ALTER TABLE document.crop RENAME COLUMN cropname TO crop_name;
ALTER TABLE document.crop RENAME COLUMN nitrogencover TO nitrogen_cover;
ALTER TABLE document.crop RENAME COLUMN nitrogenfoundation TO nitrogen_foundation;
ALTER TABLE document.crop RENAME COLUMN minv TO min_v;
ALTER TABLE document.crop RENAME COLUMN extradata TO extra_data;

--Alter for document.crop_protocol
ALTER TABLE document.crop_protocol RENAME COLUMN cropid TO crop_id;
ALTER TABLE document.crop_protocol RENAME COLUMN protocolid TO protocol_id;

-- Alter for document.fertilizer
ALTER TABLE document.fertilizer RENAME COLUMN fertilizerid TO fertilizer_id;
ALTER TABLE document.fertilizer RENAME COLUMN isavailable TO is_available;

--Alter for inventory.hazard
ALTER TABLE inventory.hazard RENAME COLUMN hazardid TO hazard_id;
ALTER TABLE inventory.hazard RENAME COLUMN hazardclass TO hazard_class;
ALTER TABLE inventory.hazard RENAME COLUMN hazardname TO hazard_name;

--Alter for parameters.parameter
ALTER TABLE parameters.parameter RENAME COLUMN parameterid TO parameter_id;
ALTER TABLE parameters.parameter RENAME COLUMN catalogid TO catalog_id;
ALTER TABLE parameters.parameter RENAME COLUMN parametername TO parameter_name;
ALTER TABLE parameters.parameter RENAME COLUMN inputquantity TO input_quantity;
ALTER TABLE parameters.parameter RENAME COLUMN officialdoc TO official_doc;

--Alter for customers.partner
ALTER TABLE customers.partner RENAME COLUMN partnerid TO partner_id;
ALTER TABLE customers.partner RENAME COLUMN partnername TO partner_name;
ALTER TABLE customers.partner RENAME COLUMN officename TO office_name;
ALTER TABLE customers.partner RENAME COLUMN partnerphone TO partner_phone;
ALTER TABLE customers.partner RENAME COLUMN partneremail TO partner_email;

--Alter users.permission
ALTER TABLE users.permission RENAME COLUMN permissionid TO permission_id;
ALTER TABLE users.permission RENAME COLUMN roleid TO role_id;
ALTER TABLE users.permission RENAME COLUMN cashflow TO cash_flow;

-- Alter customers.property
ALTER TABLE customers.property RENAME COLUMN propertyid TO property_id;
ALTER TABLE customers.property RENAME COLUMN clientid TO client_id;
ALTER TABLE customers.property RENAME COLUMN stateid TO state_id;
ALTER TABLE customers.property RENAME COLUMN propertyname TO property_name;
ALTER TABLE customers.property RENAME COLUMN postalcode TO postal_code;
ALTER TABLE customers.property RENAME COLUMN itrnirf TO itr_nirf;

--Alter document.protocol
ALTER TABLE document.protocol RENAME COLUMN protocolid TO protocol_id;
ALTER TABLE document.protocol RENAME COLUMN cashflowid TO cash_flow_id;
ALTER TABLE document.protocol RENAME COLUMN reportid TO report_id;
ALTER TABLE document.protocol RENAME COLUMN clientid TO client_id;
ALTER TABLE document.protocol RENAME COLUMN propertyid TO property_id;
ALTER TABLE document.protocol RENAME COLUMN partnerid TO partner_id;
ALTER TABLE document.protocol RENAME COLUMN catalogid TO catalog_id;
ALTER TABLE document.protocol RENAME COLUMN entrydate TO entry_date;
ALTER TABLE document.protocol RENAME COLUMN reportdate TO report_date;
ALTER TABLE document.protocol RENAME COLUMN iscollectedbyclient TO is_collected_by_client;

--Alter document.report
ALTER TABLE document.report RENAME COLUMN reportid TO report_id;
ALTER TABLE document.report RENAME COLUMN protocolid TO protocol_id;

--Alter users.role
ALTER TABLE users.role RENAME COLUMN roleid TO role_id;
ALTER TABLE users.role RENAME COLUMN rolename TO role_name;

--Alter cash_flow.transaction
ALTER TABLE cash_flow.transaction RENAME COLUMN transactionid TO transaction_id;
ALTER TABLE cash_flow.transaction RENAME COLUMN transactiontype TO transaction_type;
ALTER TABLE cash_flow.transaction RENAME COLUMN ownername TO owner_name;
ALTER TABLE cash_flow.transaction RENAME COLUMN bankname TO bank_name;

--Alter utils.state
ALTER TABLE utils.state RENAME COLUMN stateid TO state_id;
ALTER TABLE utils.state RENAME COLUMN statename TO state_name;
ALTER TABLE utils.state RENAME COLUMN statecode TO state_code;


DROP SCHEMA IF EXISTS employee CASCADE;
CREATE SCHEMA employee;

DROP TABLE IF EXISTS employee.employee;
CREATE TABLE employee.employee(
    employee_id        UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    user_id            UUID REFERENCES users.user(user_id) NOT NULL,
    name               VARCHAR(100) NOT NULL
);

INSERT INTO 
    employee.employee(user_id,name)
VALUES
(
    (SELECT user_id FROM users.user WHERE username = 'labsolo'),
    'Daniel Muniz'
),
(
    (SELECT user_id FROM users.user WHERE username = 'valter'),
    'Valter Lima'
),
(
    (SELECT user_id FROM users.user WHERE username = 'lucas'),
    'Lucas Muniz'
),
(
    (SELECT user_id FROM users.user WHERE username = 'fatima'),
    'Fátima Muniz'
);

ALTER TABLE customers.partner
ADD COLUMN user_id UUID REFERENCES users.user(user_id);

UPDATE customers.partner cp
    SET user_id = u.user_id
FROM
    users.user AS u
WHERE
    cp.partner_id = u.partner_id;

ALTER TABLE customers.client
ADD COLUMN user_id UUID REFERENCES users.user(user_id);

UPDATE customers.client AS cc
    SET user_id = u.user_id
FROM
    users.user AS u
WHERE
    cc.client_tax_id = u.username;

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