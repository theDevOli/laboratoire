-- SELECT
--     *
-- FROM customers.partner
-- WHERE partnername ='ad';

-- SELECT 
--     *
-- FROM users.user

-- SELECT 
--     *
-- FROM customers.property
-- WHERE
--     property.area LIKE'.%';
-- ROLLBACK;

-- SELECT
--     COUNT(*) AS Count
-- FROM document.crop_protocol
-- GROUP BY protocolId
-- HAVING  COUNT(*)>1
-- -- DELETE FROM document.crop_protocol

-- SELECT
--     *
-- FROM document.protocol
-- WHERE protocol.entrydate BETWEEN '2025-01-01' AND '2025-02-01';

-- UPDATE document.protocol
-- SET
--     entrydate = 2,
--     reportdate =
-- WHERE protocol.protocolid =

-- SELECT * FROM cash_flow.cash_flow WHERE cash_flow.cashflowid =1
-- SELECT * FROM cash_flow.transaction


-- SELECT
--     EXTRACT(MONTH FROM p.entrydate),
--     cp.PartnerName,
--     COUNT(*) AS "Quantidade"
-- FROM document.protocol AS p
-- INNER JOIN customers.partner AS cp
-- ON p.PartnerId = cp.PartnerId
-- WHERE p.EntryDate BETWEEN '2025-01-01' AND '2025-12-30'
-- GROUP BY EXTRACT(MONTH FROM p.entrydate), cp.PartnerName
-- ORDER BY EXTRACT(MONTH FROM p.entrydate), COUNT(*) DESC

-- SELECT
--     cp.PartnerName,
--     COUNT(*) AS "Quantidade"
-- FROM document.protocol AS p
-- INNER JOIN customers.partner AS cp
-- ON p.PartnerId = cp.PartnerId
-- WHERE p.EntryDate BETWEEN '2025-01-01' AND '2025-12-30'
-- AND p.cashflowid IS NULL
-- GROUP BY cp.PartnerName
-- ORDER BY COUNT(*) DESC
-- DELETE FROM document.protocol

-- SELECT
--     *
-- FROM cash_flow.cash_flow
-- DELETE FROM cash_flow.cash_flow

-- SELECT
--     *
-- FROM customers.client
-- WHERE client.clientid = 'ac9324e4-e620-42fe-964f-8b400351d3b2'

-- SELECT
--     *
-- FROM customers.property
-- WHERE area is null
-- BEGIN;
-- UPDATE customers.property
-- SET
--     area = null
-- WHERE area LIKE '%..%';
-- COMMIT;

-- SELECT
--     city,
--     COUNT(*) AS c
-- FROM customers.property
-- GROUP BY city
-- ORDER BY c DESC

-- DELETE FROM customers.property

-- SELECT
--     *
-- FROM utils.state

-- SELECT 
--     *
-- FROM customers.client
-- WHERE client.clienttaxid LIKE '%000000%'
-- ORDER BY client.clienttaxid DESC

-- SELECT 
--     username,
--     COUNT(*)
-- FROM users.user
-- WHERE roleId = 4
-- GROUP BY username
-- ORDER BY username
-- ;

-- SELECT  
--     *
-- FROM cash_flow.cash_flow
-- ORDER BY CashFlowId DESC;


-- SELECT
--     *
-- FROM
--     parameters.parameter
-- WHERE
--     vmp LIKE '%>%';

-- BEGIN;
-- UPDATE customers.property
--     SET postalcode = '57700000'
-- WHERE city LIKE 'Viçosa'

-- COMMIT;
-- SELECT 
--     cp.city,
--     COUNT(*),
--     s.statename,
--     s.stateid,
--     cp.postalcode
-- FROM customers.property AS cp
-- INNER Join utils.state AS s
-- ON cp.stateid = s.stateid
-- WHERE city <>'*****'
-- AND cp.postalcode IS NULL
-- GROUP BY cp.city, s.statename,s.stateid,cp.postalcode
-- ORDER BY cp.city;

-- ROLLBACK;

-- BEGIN;
-- UPDATE customers.property
--     SET city = 'Teofilândia'
--     -- SET stateId = 5
-- WHERE city LIKE 'Teofilandia';

-- COMMIT;

-- SELECT 
--     COUNT(*),
--     city,
--     stateid,

-- FROM customers.property
-- WHERE city LIKE '%Saloa%'
-- -- AND postalcode Is NOT NULL
-- GROUP BY city,stateid

-- DROP DATABASE labsolo_db

-- SELECT * FROM logs;
-- SELECT 
--     r.rolename
-- FROM
--     users.user AS u
-- INNER JOIN
--     users.role AS r 
--     ON u.roleid = r.roleid
-- WHERE
--     UserId = 'e5c7cd98-4186-485f-8045-eb68981c8e7e';


-- SELECT
--             u.user_id AS UserId,
--             u.username AS Username,
--             u.is_active AS IsActive,

--             r.role_id AS RoleId,
--             r.role_name AS RoleName,

--             COALESCE(cc.client_name, cp.partner_name, e.name) AS Name
--         FROM 
--             users.user AS u
--         INNER JOIN 
--             users.role AS r 
--             ON r.role_id = u.role_Id
--         LEFT JOIN 
--             customers.client AS cc 
--             ON u.user_id = cc.user_id
--         LEFT JOIN 
--             customers.partner AS cp
--             ON u.user_id = cp.user_id
--          LEFT JOIN 
--             employee.employee AS e
--             ON u.user_id = e.user_id
--         ORDER BY 
--             r.role_id,u.username

-- SELECT
--         ua.user_id AS UserId,
--         ua.password_salt AS PasswordSalt,
--         ua.password_hash AS PasswordHash,
--         COALESCE(cc.client_name, cp.partner_name, e.name) AS Name
--     FROM 
--         users.auth AS ua
--      LEFT JOIN 
--             customers.client AS cc 
--             ON ua.user_id = cc.user_id
--         LEFT JOIN 
--             customers.partner AS cp
--             ON ua.user_id = cp.user_id
--          LEFT JOIN 
--             employee.employee AS e
--             ON ua.user_id = e.user_id
--     WHERE 
--         ua.user_id = '8964efe3-2ceb-4581-8eb4-8531570c8bea';

SELECT
    cp.partner_id AS PartnerId,
    cp.partner_name AS PartnerName,
    cp.office_name AS OfficeName,
    cp.partner_phone AS PartnerPhone,
    cp.partner_email AS PartnerEmail
FROM 
    customers.partner AS cp
INNER JOIN
    users.user AS u 
    ON cp.user_id = u.user_id
WHERE
    u.is_active = true
ORDER BY
    cp.partner_name;
