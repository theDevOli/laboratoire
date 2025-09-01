SELECT table_schema, table_name
FROM information_schema.tables
WHERE table_type = 'BASE TABLE'
  AND table_schema NOT IN ('pg_catalog', 'information_schema');

SELECT 
    *
FROM public.tb_laudo
WHERE id_protocolo =156;

SELECT 
    tp.id_tipo_plantio,
    tp.descricao
FROM public.tb_tipo_plantio AS tp
ORDER By tp.id_tipo_plantio;

SELECT 
  concat_ws(
    '\', 
    p.numero_protocolo,
    EXTRACT(YEAR FROM p.data_ensaio)
  ) AS protocolo_ano,
  p.id_projetista
FROM public.tb_protocolo AS p
WHERE p.id_projetista IS NOT NULL;

INNER JOIN public.tb_projetista AS pr 
ON pr.id_projetista= p.id_projetista
WHERE p.id_projetista IS NOT NULL
ORDER BY data_ensaio, numero_protocolo;


SELECT 
    p.id_protocolo,
    p.numero_protocolo,
    p.data_ensaio,
    p.data_entrega,
    p.situacao,
    p.id_tipo_plantio,
    p.id_propriedade,
    p.id_projetista,
    p.is_replicado,
    pr.proprietario
FROM public.tb_protocolo AS p
INNER JOIN public.tb_propriedade AS pr 
ON pr.id_propriedade = p.id_propriedade
ORDER BY p.data_ensaio,p.numero_protocolo;

SELECT 
    *
FROM public.tb_protocolo_replicado;

SELECT 
    *
FROM public.tb_laudo;
-- WHERE data_ensaio BETWEEN '2025-01-01' AND '2025-12-01'
-- ORDER BY numero_protocolo;

SELECT 
    *
FROM public.tb_cliente
WHERE id_cliente = 254;
-- WHERE cpf ='83940367591';

SELECT 
    *
FROM public.tb_propriedade

SELECT * FROM tb_projetista