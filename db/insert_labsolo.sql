INSERT INTO parameters.catalog(
    ReportType,
    SampleType,
    LabelName,
    Legends,
    Price
)
VALUES
(
    'Física',
    'Solo',
    'Análise Física',
    ARRAY[
        '{"unit":"Tipo 1","description":"Solo de textura arenosa"}'::JSONB,
        '{"unit":"Tipo 2","description":"Solo de textura media"}'::JSONB,
        '{"unit":"Tipo 3","description":"Solo de textura argilosa"}'::JSONB,
        '{"unit":"AD1","description":"Água Disponível 1"}'::JSONB,
        '{"unit":"AD2","description":"Água Disponível 2"}'::JSONB,
        '{"unit":"AD3","description":"Água Disponível 3"}'::JSONB,
        '{"unit":"AD4","description":"Água Disponível 4"}'::JSONB,
        '{"unit":"AD5","description":"Água Disponível 5"}'::JSONB,
        '{"unit":"AD6","description":"Água Disponível 6"}'::JSONB
   ],
    30.00
),
(
    'Físico-Química',
    'Solo',
    'Análise Físico-Química',
    ARRAY[
        '{"unit":"dag/kg","description":"Decagrama por kilo"}'::JSONB,
        '{"unit":"cmol<sub>c</sub>/kg","description":"Centimol por kilo"}'::JSONB,
        '{"unit":"mg/L","description":"Miligrama por litro"}'::JSONB,
        '{"unit":"mm/cm","description":"Milímetros por centímetros"}'::JSONB,
        '{"unit":"cmol<sub>c</sub>/L","description":"Centimol por litro"}'::JSONB,
        '{"unit":"Tipo 1","description":"Solo de textura arenosa"}'::JSONB,
        '{"unit":"Tipo 2","description":"Solo de textura media"}'::JSONB,
        '{"unit":"Tipo 3","description":"Solo de textura argilosa"}'::JSONB,
        '{"unit":"AD1","description":"Água Disponível 1"}'::JSONB,
        '{"unit":"AD2","description":"Água Disponível 2"}'::JSONB,
        '{"unit":"AD3","description":"Água Disponível 3"}'::JSONB,
        '{"unit":"AD4","description":"Água Disponível 4"}'::JSONB,
        '{"unit":"AD5","description":"Água Disponível 5"}'::JSONB,
        '{"unit":"AD6","description":"Água Disponível 6"}'::JSONB
   ],
    80.00
),
(
    'Química',
    'Solo',
    'Análise Química',
    ARRAY[
        '{"unit":"ND","description":"Não detectado"}'::JSONB,
        '{"unit":"dag/kg","description":"Decagrama por kilo"}'::JSONB,
        '{"unit":"cmol<sub>c</sub>/kg","description":"Centimol por kilo"}'::JSONB,
        '{"unit":"mg/L","description":"Miligrama por litro"}'::JSONB,
        '{"unit":"mm/cm","description":"Milímetros por centímetros"}'::JSONB,
        '{"unit":"cmol<sub>c</sub>/L","description":"Centimol por litro"}'::JSONB
   ],
    60.00
),
(
    'Microbiologia',
    'Água',
    'Análise Microbiologica',
    ARRAY[
    '{"unit":"ND","description":"Não detectado"}'::JSONB,
    '{"unit":"VMP","description":"Valor Maximo Permitido"}'::JSONB,
    '{"unit":"UFC/100mL","description":"Unidades Formadoras de Colonias em 100 mililitros"}'::JSONB
   ],
    70.00
),
(
    'Potabilidade',
    'Água',
    'Análise de Potabilidade',
    ARRAY[
        '{"unit":"ND","description":"Não detectado"}'::JSONB,
        '{"unit":"VMP","description":"Valor Maximo Permitido"}'::JSONB,
        '{"unit":"UFC/100mL","description":"Unidades Formadoras de Colonias em 100 mililitros"}'::JSONB,
        '{"unit":"mg/L","description":"Miligrama por litro"}'::JSONB,
        '{"unit":"µS/cm","description":"MicroSiemens por centimetro"}'::JSONB,
        '{"unit":"uH","description":"Unidade Hazen (mg Pt-Co/L)"}'::JSONB,
        '{"unit":"NTU","description":"Unidade de Turbidez Nefrolométrica"}'::JSONB,
        '{"unit":"°C","description":"Graus Celsius "}'::JSONB
   ],
    250.00
),
(
    'Irrigação',
    'Água',
    'Análise de Irrigação',
    ARRAY[
        '{"unit":"ND","description":"Não detectado"}'::JSONB,
        '{"unit":"VMP","description":"Valor Maximo Permitido"}'::JSONB,
        '{"unit":"UFC/100mL","description":"Unidades Formadoras de Colonias em 100 mililitros"}'::JSONB,
        '{"unit":"mg/L","description":"Miligrama por litro"}'::JSONB,
        '{"unit":"µS/cm","description":"MicroSiemens por centimetro"}'::JSONB,
        '{"unit":"uH","description":"Unidade Hazen (mg Pt-Co/L)"}'::JSONB,
        '{"unit":"NTU","description":"Unidade de Turbidez Nefrolométrica"}'::JSONB,
        '{"unit":"°C","description":"Graus Celsius"}'::JSONB,
        '{"unit":"(meq/L)<sup>1/2</sup>","description":"Raiz quadrada de miliequivalente"}'::JSONB
   ],
    200.00
),
(
    'Água',
    'Potabilidade e Irrigação',
    'Análise de Potabilidade e Irrigação',
    ARRAY[
        '{"unit":"ND","description":"Não detectado"}'::JSONB,
        '{"unit":"VMP","description":"Valor Maximo Permitido"}'::JSONB,
        '{"unit":"mg/L","description":"Miligrama por litro"}'::JSONB,
        '{"unit":"µS/cm","description":"MicroSiemens por centimetro"}'::JSONB,
        '{"unit":"°C","description":"Graus Celsius"}'::JSONB,
        '{"unit":"(meq/L)<sup>1/2</sup>","description":"Raiz quadrada de miliequivalente"}'::JSONB
   ],
    300.00
),
(
    'Potabilidade para Rebanho Bovino',
    'Água',
    'Análise de Gado',
    ARRAY[
        '{"unit":"ND","description":"Não detectado"}'::JSONB,
        '{"unit":"VMP","description":"Valor Maximo Permitido"}'::JSONB,
        '{"unit":"UFC/100mL","description":"Unidades Formadoras de Colonias em 100 mililitros"}'::JSONB,
        '{"unit":"mg/L","description":"Miligrama por litro"}'::JSONB,
        '{"unit":"°C","description":"Graus Celsius "}'::JSONB
   ],
    180.00
),
(
    'Monitoramento Ambiental',
    'Efluentes',
    'Efluente Bruto',
    ARRAY[
        '{"unit":"ND","description":"Não detectado"}'::JSONB,
        '{"unit":"VMP","description":"Valor Maximo Permitido"}'::JSONB,
        '{"unit":"UFC/100mL","description":"Unidades Formadoras de Colonias em 100 mililitros"}'::JSONB,
        '{"unit":"mg/L","description":"Miligrama por litro"}'::JSONB,
        '{"unit":"uH","description":"Unidade Hazen (mg Pt-Co/L)"}'::JSONB,
        '{"unit":"°C","description":"Graus Celsius "}'::JSONB
   ],
    300.00
),
(
    'Monitoramento Ambiental',
    'Efluentes',
    'Efluente Tratado',
    ARRAY[
        '{"unit":"ND","description":"Não detectado"}'::JSONB,
        '{"unit":"VMP","description":"Valor Maximo Permitido"}'::JSONB,
        '{"unit":"UFC/100mL","description":"Unidades Formadoras de Colonias em 100 mililitros"}'::JSONB,
        '{"unit":"mg/L","description":"Miligrama por litro"}'::JSONB,
        '{"unit":"uH","description":"Unidade Hazen (mg Pt-Co/L)"}'::JSONB,
        '{"unit":"°C","description":"Graus Celsius "}'::JSONB
   ],
    300.00
);

INSERT INTO parameters.parameter (
    CatalogId,
    ParameterName,
    Unit,
    InputQuantity,
    OfficialDoc,
    Vmp,
    Equation
)
VALUES
(
    -- 1
    1,
    'Areia',
    '%',
    1,
    'MAQS-Embrapa',
    NULL,
    NULL    
),
(
    -- 2
    1,
    'Argila',
    '%',
    1,
    'MAQS-Embrapa',
    NULL,
    NULL    
),
(
    -- 3
    1,
    'Silte',
    '%',
    0,
    'MAQS-Embrapa',
    NULL,
    NULL    
),
(
    -- 4
    1,
    'Classificação Textural (Triângulo Americano)',
    NULL,
    0,
    'MAPA-IN nº. 02 de 09/11/21',
    NULL,
    NULL    
),
(
    -- 5
    1,
    'Água Disponível (AD)',
    'mm/cm',
    0,
    'MAPA-IN nº. 02 de 05/08/22',
    NULL,
    NULL    
),
(
    -- 6
    1,
    'Classificação Água Disponível',
    NULL,
    0,
    'MAPA-IN nº. 02 de 05/08/22',
    NULL,
    NULL    
),
(
    -- 7
    2,
    'pH em Água',
    NULL,
    1,
    'MAQS-Embrapa',
    NULL,
    NULL    
),
(
    -- 8
    2,
    'Matéria Orgânica',
    'dag/kg',
    1,
    'MAQS-Embrapa',
    NULL,
    NULL    
),
(
    -- 9
    2,
    'Cálcio + Magnésio',
    'cmol<sub>c</sub>/kg',
    1,
    'MAQS-Embrapa',
    NULL,
    NULL    
),
(
    -- 10
    2,
    'Cálcio',
    'cmol<sub>c</sub>/kg',
    1,
    'MAQS-Embrapa',
    NULL,
    NULL    
),
(
    -- 11
    2,
    'Magnésio',
    'cmol<sub>c</sub>/kg',
    0,
    'MAQS-Embrapa',
    NULL,
    NULL    
),
(
    -- 12
    2,
    'Alumínio',
    'cmol<sub>c</sub>/kg',
    1,
    'MAQS-Embrapa',
    NULL,
    NULL   
),
(
    -- 13
    2,
    'Hidrogênio + Alumínio',
    'cmol<sub>c</sub>/kg',
    2,
    'MAQS-Embrapa',
    NULL,
    'x*1.65'  
),
(
    -- 14
    2,
    'Sódio',
    'mg/L',
    1,
    'MAQS-Embrapa',
    NULL,
    NULL    
),
(
    -- 15
    2,
    'Potássio',
    'mg/L',
    1,
    'MAQS-Embrapa',
    NULL,
    NULL    
),
(
    -- 16
    2,
    'Fósforo',
    'mg/L',
    1,
    'MAQS-Embrapa',
    NULL,
    '(x+0.0082)/0.0989'    
),
(
    -- 17
    2,
    'SB- Soma de Bases Trocáveis',
    'cmol<sub>c</sub>/L',
    0,
    'MAQS-Embrapa',
    NULL,
    NULL    
),
(
    -- 18
    2,
    'CTC',
    'cmol<sub>c</sub>/L',
    0,
    'MAQS-Embrapa',
    NULL,
    NULL    
),
(
    -- 19
    2,
    'V-Índice de Saturação de Bases',
    '%',
    0,
    'MAQS-Embrapa',
    NULL,
    NULL    
),
(
    -- 20
    2,
    'Areia',
    '%',
    1,
    'MAQS-Embrapa',
    NULL,
    NULL    
),
(
    2,
    'Argila',
    '%',
    1,
    'MAQS-Embrapa',
    NULL,
    NULL    
),
(
    2,
    'Silte',
    '%',
    0,
    'MAQS-Embrapa',
    NULL,
    NULL    
),
(
    2,
    'Classificação Textural (Triângulo Americano)',
    NULL,
    0,
    'MAPA-IN nº. 02 de 09/11/21',
    NULL,
    NULL    
),
(
    2,
    'Água Disponível (AD)',
    'mm/cm',
    0,
    'MAPA-IN nº. 02 de 05/08/22',
    NULL,
    NULL    
),
(
    2,
    'Classificação Água Disponível',
    NULL,
    0,
    'MAPA-IN nº. 02 de 05/08/22',
    NULL,
    NULL    
),
(
    3,
    'pH em Água',
    NULL,
    1,
    'MAQS-Embrapa',
    NULL,
    NULL    
),
(
    3,
    'Matéria Orgânica',
    'dag/kg',
    1,
    'MAQS-Embrapa',
    NULL,
    NULL    
),
(
    3,
    'Cálcio + Magnésio',
    'cmol<sub>c</sub>/kg',
    1,
    'MAQS-Embrapa',
    NULL,
    NULL    
),
(
    3,
    'Cálcio',
    'cmol<sub>c</sub>/kg',
    1,
    'MAQS-Embrapa',
    NULL,
    NULL    
),
(
    3,
    'Magnésio',
    'cmol<sub>c</sub>/kg',
    0,
    'MAQS-Embrapa',
    NULL,
    NULL    
),
(
    3,
    'Alumínio',
    'cmol<sub>c</sub>/kg',
    1,
    'MAQS-Embrapa',
    NULL,
    NULL    
),
(
    3,
    'Hidrogênio + Alumínio',
    'cmol<sub>c</sub>/kg',
    1,
    'MAQS-Embrapa',
    NULL,
    'x*1.65'    
),
(
    3,
    'Sódio',
    'mg/L',
    1,
    'MAQS-Embrapa',
    NULL,
    NULL    
),
(
    3,
    'Potássio',
    'mg/L',
    1,
    'MAQS-Embrapa',
    NULL,
    NULL    
),
(
    3,
    'Fósforo',
    'mg/L',
    1,
    'MAQS-Embrapa',
    NULL,
    '(x+0.0082)/0.0989'
),
(
    3,
    'SB- Soma de Bases Trocáveis',
    'cmol<sub>c</sub>/L',
    0,
    'MAQS-Embrapa',
    NULL,
    NULL    
),
(
    3,
    'CTC',
    'cmol<sub>c</sub>/L',
    0,
    'MAQS-Embrapa',
    NULL,
    NULL    
),
(
    3,
    'V-Índice de Saturação de Bases',
    '%',
    0,
    'MAQS-Embrapa',
    NULL,
    NULL    
),
(
    4,
    'Coliformes Totais',
    'UFC/100mL',
    1,
    'Portaria GM/MS Nº 888, de 04/05/2021',
    '0',
    'x*40'
),
(
    4,
    'Escherichia Coli',
    'UFC/100mL',
    1,
    'Portaria GM/MS Nº 888, de 04/05/2021',
    '0',
    'x*40'
),
(
    5,
    'Alcalinidade',
    'mg/L',
    1,
    NULL,
    NULL,
    'x*20'
),
(
    5,
    'Alumínio',
    'mg/L',
    1,
    'Portaria GM/MS Nº 888, de 04/05/2021',
    '0.2',
    'x/0.697695852534562' 
),
(
    5,
    'Cloro Residual Livre',
    'mg/L',
    1,
    'Portaria GM/MS Nº 888, de 04/05/2021',
    '> 0.2',
    NULL
),
(
    5,
    'Condutividade Elétrica',
    'µS/cm',
    1,
    NULL,
    NULL,
    NULL
),
(
    5,
    'Cor Aparente',
    'uH',
    1,
    'Portaria GM/MS Nº 888, de 04/05/2021',
    '15',
    NULL
),
(
    5,
    'Dióxido de Carbono',
    'mg/L',
    1,
    NULL,
    NULL,
    '(x*22)*0.988'
),
(
    5,
    'Dureza Total',
    'mg/L',
    1,
    'Portaria GM/MS Nº 888',
    '300',
    'x*100'
),
(
    5,
    'Ferro',
    'mg/L',
    1,
    'Portaria GM/MS Nº 888',
    '0.3',
    'x/0.0852195121951219'
),
(
    5,
    'pH',
    NULL,
    1,
    NULL,
    NULL,
    NULL
),
(
    5,
    'Sódio',
    'mg/L',
    1,
    'Portaria GM/MS Nº 888',
    '200',
    NULL
),
(
    5,
    'Sólidos Dissolvidos Totais (SDT)',
    'mg/L',
    2,
    'Portaria GM/MS Nº 888',
    '500',
    'x*20000'
),
(
    5,
    'Temperatura',
    'ºC',
    1,
    NULL,
    NULL,
    NULL
),
(
    5,
    'Turbidez',
    'NTU',
    1,
    'Portaria GM/MS Nº 888',
    '5',
    NULL
),
(
    5,
    'Coliformes Totais',
    'UFC/100mL',
    1,
    'Portaria GM/MS Nº 888, de 04/05/2021',
    '0',
    'x*40'
),
(
    5,
    'Escherichia Coli',
    'UFC/100mL',
    1,
    'Portaria GM/MS Nº 888, de 04/05/2021',
    '0',
    'x*40'
),
(
    6,
    'Alcalinidade',
    'mg/L',
    1,
    NULL,
    NULL,
    'x*20'
),
(
    6,
    'Condutividade Elétrica',
    'µS/cm',
    1,
    NULL,
    NULL,
    NULL
),
(
    6,
    'Dureza Total',
    'mg/L',
    1,
    NULL,
    NULL,
    'x*100'
),
(
    6,
    'pH',
    NULL,
    1,
    NULL,
    NULL,
    NULL
),
(
    6,
    'RAS',
    '(meq/L)<sup>1/2</sup>',
    0,
    NULL,
    NULL,
    NULL
),
(
    6,
    'Salinidade',
    NULL,
    0,
    NULL,
    NULL,
    NULL
),
(
    6,
    'Sodicidade',
    NULL,
    0,
    NULL,
    NULL,
    NULL
),
(
    6,
    'Sódio',
    'mg/L',
    1,
    NULL,
    NULL,
    NULL
),
(
    6,
    'Sólidos Dissolvidos Totais (SDT)',
    'mg/L',
    2,
    NULL,
    NULL,
    'x*20000'
),
(
    6,
    'Sulfatos',
    'mg/L',
    1,
    NULL,
    NULL,
    'x/0.00995052631578947'
),
(
    6,
    'Temperatura',
    'ºC',
    1,
    NULL,
    NULL,
    NULL
),
(
    7,
    'Alcalinidade',
    'mg/L',
    1,
    NULL,
    NULL,
    'x*20'
),
(
    7,
    'Alumínio',
    'mg/L',
    1,
    'Portaria GM/MS Nº 888, de 04/05/2021',
    '0.2',
    'x/0.697695852534562' 
),
(
    7,
    'Cloro Residual Livre',
    'mg/L',
    1,
    'Portaria GM/MS Nº 888, de 04/05/2021',
    '> 0.2',
    NULL
),
(
    7,
    'Condutividade Elétrica',
    'µS/cm',
    1,
    NULL,
    NULL,
    NULL
),
(
    7,
    'Cor Aparente',
    'uH',
    1,
    'Portaria GM/MS Nº 888, de 04/05/2021',
    '15',
    NULL
),
(
    7,
    'Dióxido de Carbono',
    'mg/L',
    1,
    NULL,
    NULL,
    '(x*22)*0.988'
),
(
    7,
    'Dureza Total',
    'mg/L',
    1,
    NULL,
    NULL,
    'x*100'
),
(
    7,
    'Ferro',
    'mg/L',
    1,
    'Portaria GM/MS Nº 888',
    '0.3',
    'x/0.0852195121951219'
),
(
    7,
    'pH',
    NULL,
    1,
    NULL,
    NULL,
    NULL
),
(
   7,
    'RAS',
    '(meq/L)<sup>1/2</sup>',
    0,
    NULL,
    NULL,
    NULL 
),
(
    7,
    'Salinidade',
    NULL,
    0,
    NULL,
    NULL,
    NULL
),
(
    7,
    'Sodicidade',
    NULL,
    0,
    NULL,
    NULL,
    NULL
),
(
    7,
    'Sódio',
    'mg/L',
    1,
    'Portaria GM/MS Nº 888',
    '200',
    NULL
),
(
    7,
    'Sólidos Dissolvidos Totais (SDT)',
    'mg/L',
    2,
    'Portaria GM/MS Nº 888',
    '500',
    'x*20000'
),
(
    7,
    'Sulfatos',
    'mg/L',
    1,
    NULL,
    NULL,
    'x/0.00995052631578947'
),
(
    7,
    'Temperatura',
    'ºC',
    1,
    NULL,
    NULL,
    NULL
),
(
    7,
    'Turbidez',
    'NTU',
    1,
    'Portaria GM/MS Nº 888',
    '5',
    NULL
),
(
    7,
    'Coliformes Totais',
    'UFC/100mL',
    1,
    'Portaria GM/MS Nº 888, de 04/05/2021',
    '0',
    'x*40'
),
(
    7,
    'Escherichia Coli',
    'UFC/100mL',
    1,
    'Portaria GM/MS Nº 888, de 04/05/2021',
    '0',
    'x*40'
),
(
    8,
    'Cloretos',
    'mg/L',
    1,
    NULL,
    NULL,
    NULL
),
(
    8,
    'pH',
    NULL,
    1,
    NULL,
    NULL,
    NULL
),
(
    8,
    'Nitrato',
    'mg/L',
    1,
    'Resolução CONAMA Nº 396, de 03/04/2008',
    '90',
    NULL
),
(
    8,
    'Sulfatos',
    'mg/L',
    1,
    'Resolução CONAMA Nº 396, de 03/04/2008',
    '1000',
    'x/0.00995052631578947'
),
(
    8,
    'Sólidos Dissolvidos Totais (SDT)',
    'mg/L',
    2,
    'Portaria GM/MS Nº 888',
    '500',
    'x*20000'
),
(
    8,
    'Temperatura',
    'ºC',
    1,
    NULL,
    NULL,
    NULL
),
(
    8,
    'Escherichia Coli',
    'UFC/100mL',
    1,
    'Resolução CONAMA Nº 396, de 03/04/2008',
    '200',
    'x*40'
),
(
   8,
    'Coliformes Totais',
    'UFC/100mL',
    1,
    'Resolução CONAMA Nº 396, de 03/04/2008',
    '200',
    'x*40' 
),
(
    9,
    'Cloreto Total',
    'mg/L',
    1,
    'Resolução CONAMA Nº 430, de 13/05/2011',
    '250',
    'x*35.5'
),
(
    9,
    'Cor Verdadeira',
    'uH',
    1,
    'Resolução CONAMA Nº 430, de 13/05/2011',
    '75',
    NULL 
),
(
    9,
    'DBO',
    'mg/L',
    3,
    'Resolução CONAMA Nº 430, de 13/05/2011',
    NULL,
    '(i-f)*300/v' 
),
(
    9,
    'DQO',
    'mg/L',
    1,
    NULL,
    NULL,
    'x/0.000320469973890339' 
),
(
    9,
    'Fósforo',
    'mg/L',
    1,
    'Resolução CONAMA Nº 430, de 13/05/2011',
    NULL,
    NULL 
),
(
    9,
    'Nitrogênio Amoniacal',
    'mg/L',
    1,
    'Resolução CONAMA Nº 430, de 13/05/2011',
    '20',
    'x/0.634766807995155' 
),
(
    9,
    'Óleos e Graxas',
    'mg/L',
    3,
    'Resolução CONAMA Nº 430, de 13/05/2011',
    '50',
    '(f-i)*1000000/v' 
),
(
    9,
    'pH',
    NULL,
    1,
    'Resolução CONAMA Nº 430, de 13/05/2011',
    '5-9',
    NULL 
),
(
    9,
    'Sólidos Sedimentáveis',
    'mg/L',
    1,
    'Resolução CONAMA Nº 430, de 13/05/2011',
    '250',
    NULL 
),
(
    9,
    'Temperatura',
    '°C',
    1,
    'Resolução CONAMA Nº 430, de 13/05/2011',
    '40',
    NULL 
),
(
    9,
    'Escherichia Coli',
    'UFC/100mL',
    1,
    'Resolução CONAMA Nº 430, de 13/05/2011',
    '200',
    'x*40' 
),
(
    9,
    'Coliformes Totais',
    'UFC/100mL',
    1,
    'Resolução CONAMA Nº 430, de 13/05/2011',
    '200',
    'x*40' 
),
(
    10,
    'Cloreto Total',
    'mg/L',
    1,
    'Resolução CONAMA Nº 430, de 13/05/2011',
    '250',
    'x*35.5'
),
(
    10,
    'Cor Verdadeira',
    'uH',
    1,
    'Resolução CONAMA Nº 430, de 13/05/2011',
    '75',
    NULL 
),
(
    10,
    'DBO',
    'mg/L',
    3,
    'Resolução CONAMA Nº 430, de 13/05/2011',
    NULL,
    '(i-f)*300/v' 
),
(
    10,
    'Eficiência da DBO',
    '%',
    1,
    'Resolução CONAMA Nº 430, de 13/05/2011',
    '> 60',
    '(i-f)/f' 
),
(
    10,
    'DQO',
    'mg/L',
    1,
    NULL,
    NULL,
    'x/0.000320469973890339' 
),
(
    10,
    'Fósforo',
    'mg/L',
    1,
    'Resolução CONAMA Nº 430, de 13/05/2011',
    NULL,
    NULL 
),
(
    10,
    'Nitrogênio Amoniacal',
    'mg/L',
    1,
    'Resolução CONAMA Nº 430, de 13/05/2011',
    '20',
    'x/0.634766807995155' 
),
(
    10,
    'Óleos e Graxas',
    'mg/L',
    3,
    'Resolução CONAMA Nº 430, de 13/05/2011',
    '50',
    '(f-i)*1000000/v' 
),
(
    10,
    'pH',
    NULL,
    1,
    'Resolução CONAMA Nº 430, de 13/05/2011',
    '5-9',
    NULL 
),
(
    10,
    'Sólidos Sedimentáveis',
    'mg/L',
    1,
    'Resolução CONAMA Nº 430, de 13/05/2011',
    '250',
    NULL 
),
(
    10,
    'Temperatura',
    '°C',
    1,
    'Resolução CONAMA Nº 430, de 13/05/2011',
    '40',
    NULL 
),
(
    10,
    'Escherichia Coli',
    'UFC/100mL',
    1,
    'Resolução CONAMA Nº 430, de 13/05/2011',
    '200',
    'x*40' 
),
(
    10,
    'Coliformes Totais',
    'UFC/100mL',
    1,
    'Resolução CONAMA Nº 430, de 13/05/2011',
    '200',
    'x*40' 
);

INSERT INTO customers.client(ClientName,ClientTaxId)
VALUES('System_Reserved','00000000000');
    
INSERT INTO utils.state (StateName, StateCode) VALUES
('Acre', 'AC'),
('Alagoas', 'AL'),
('Amapá', 'AP'),
('Amazonas', 'AM'),
('Bahia', 'BA'),
('Ceará', 'CE'),
('Distrito Federal', 'DF'),
('Espírito Santo', 'ES'),
('Goiás', 'GO'),
('Maranhão', 'MA'),
('Mato Grosso', 'MT'),
('Mato Grosso do Sul', 'MS'),
('Minas Gerais', 'MG'),
('Pará', 'PA'),
('Paraíba', 'PB'),
('Paraná', 'PR'),
('Pernambuco', 'PE'),
('Piauí', 'PI'),
('Rio de Janeiro', 'RJ'),
('Rio Grande do Norte', 'RN'),
('Rio Grande do Sul', 'RS'),
('Rondônia', 'RO'),
('Roraima', 'RR'),
('Santa Catarina', 'SC'),
('São Paulo', 'SP'),
('Sergipe', 'SE'),
('Tocantins', 'TO');

INSERT INTO cash_flow.transaction(
    TransactionType,
    OwnerName,
    BankName
)
VALUES
('Pendente',null,null),
('Dinheiro',null,null),
('Pix','Labsolo','Inter'),
('Pix','Fátima','BB'),
('Pix','Daniel','Nubank'),
('Pix','Jairton','Nubank'),
('Boleto','Labsolo','Inter');

INSERT INTO inventory.hazard(
    HazardClass,
    HazardName
)
VALUES
('GHS01','substâncias explosivas'),
('GHS02','substâncias inflamáveis'),
('GHSO3','substâncias oxidantes'),
('GHS04','gases sob pressão'),
('GHS05','substâncias corrosivas de metais, substâncias cáusticas'),
('GHS06','substâncias tóxicas'),
('GHS07','substâncias irritantes'),
('GHS08','substâncias cancerígenas/mutagênicas'),
('GHS09','substâncias nocivas ao meio aquático');

INSERT INTO users.role(RoleName)
VALUES
('admin'),
('recepção'),
('químico'),
('projetista'),
('cliente');

INSERT INTO users.permission(
    RoleId,
    Protocol,
    Client,
    Property,
    CashFlow,
    Partner,
    Users,
    Chemical
)
VALUES
(1,TRUE,TRUE,TRUE,TRUE,TRUE,TRUE,TRUE),
(2,TRUE,TRUE,TRUE,TRUE,TRUE,FALSE,NULL),
(3,TRUE,TRUE,TRUE,FALSE,TRUE,NULL,TRUE),
(4,FALSE,NULL,NULL,FALSE,NULL,NULL,NULL),
(5,FALSE,NULL,NULL,NULL,NULL,NULL,NULL);

INSERT INTO document.fertilizer(
    Nitrogen,
    Phosphorus,
    Potassium,
    IsAvailable
)
VALUES
(
    10,
    30,
    10,
    TRUE
),
(
    9,
    40,
    5,
    TRUE
);

INSERT INTO document.crop(
    CropName,
    NitrogenCover,
    NitrogenFoundation,
    Phosphorus,
    Potassium,
    MinV,
    ExtraData
)
VALUES
(
    'Abacate',
    60,
    80,
    '{"min":150,"med":100,"max":50}',
    '{"min":60,"med":40,"max":20}',
    60,
    'Espaçamento 10 x 10 m.'
),
(
    'Abacaxi',
    NULL,
    100,
    '{"min":80,"med":60,"max":40}',
    '{"min":90,"med":75,"max":60}',
    50,
    'Espaçamento de 0,90 x 0,45 m.'
),
(
    'Abobora',
    30,
    30,
    '{"min":90,"med":60,"max":30}',
    '{"min":90,"med":60,"max":30}',
    80,
    'Espaçamento de 3,0 x 1,0 m.'
),
(
    'Acerola',
    80,
    20,
    '{"min":120,"med":80,"max":50}',
    '{"min":120,"med":80,"max":50}',
    80,
    'Espaçamento de 4,0 x 4,0 m.'
),
(
    'Alface',
    25,
    40,
    '{"min":200,"med":100,"max":50}',
    '{"min":75,"med":50,"max":25}',
    70,
    'Espaçamento de 0,3 x 0,3 m.'
),
(
    'Alho',
    30,
    30,
    '{"min":180,"med":120,"max":60}',
    '{"min":50,"med":35,"max":20}',
    70,
    'Espaçamento de 0,3 x 0,1 m.'
),
(
    'Amendoim',
    30,
    20,
    '{"min":40,"med":20,"max":0}',
    '{"min":60,"med":40,"max":0}',
    60,
    'Espaçamento: 0,50 m entrelinhas e 10 sementes por metro de sulco.'
),
(
    'Arroz',
    30,
    30,
    '{"min":100,"med":100,"max":100}',
    '{"min":150,"med":150,"max":150}',
    10,
    'A dosagem do potássio se justifica para aumentar a resistência das plantas às doenças e pragas.'
),
(
    'Banana',
    NULL,
    280,
    '{"min":110,"med":90,"max":60}',
    '{"min":450,"med":350,"max":170}',
    60,
    'Espaçamento de 2,0 x 2,0 m ou 2,0 x 2,5 m.'
),
(
    'Batata-doce',
    30,
    20,
    '{"min":90,"med":60,"max":40}',
    '{"min":100,"med":70,"max":40}',
    70,
    'Espaçamento: 0,8 x 0,3 m.'
),
(
    'Berinjela',
    40,
    40,
    '{"min":200,"med":140,"max":80}',
    '{"min":120,"med":80,"max":40}',
    80,
    'Espaçamento de 1,0 x 0,5 m.'
),
(
    'Beterraba',
    40,
    40,
    '{"min":200,"med":140,"max":80}',
    '{"min":140,"med":100,"max":60}',
    80,
    'Espaçamento 0,25 x 0,25 m.'
),
(
    'Caju',
    80,
    60,
    '{"min":60,"med":400,"max":0}',
    '{"min":90,"med":60,"max":0}',
    70,
   NULL
),
(
    'Cana-de-açúcar',
    NULL,
    80,
    '{"min":150,"med":100,"max":50}',
    '{"min":120,"med":90,"max":60}',
    60,
    'Espaçamento entre 1,2 m.'
),
(
    'Capim Buffel',
    50,
    80,
    '{"min":80,"med":60,"max":20}',
    '{"min":60,"med":40,"max":20}',
    70,
    'Espaçamento entre sulcos de 0,9 m.'
),
(
    'Capim Pangola',
    50,
    80,
    '{"min":80,"med":60,"max":20}',
    '{"min":60,"med":40,"max":20}',
    70,
    'Espaçamento entre sulcos de 0,9 m.'
),
(
    'Cebola',
    40,
    40,
    '{"min":160,"med":120,"max":80}',
    '{"min":160,"med":120,"max":80}',
    80,
    'Espaçamento 0,1 x 0,1 m.'
),
(
    'Cebolinha ou coentro',
    12,
    6,
    '{"min":30,"med":24,"max":20}',
    '{"min":12,"med":6,"max":4}',
    80,
    'Espaçamentos da cebolinha: 0,2 x 0,1 m; 
Espaçamentos do coentro: sulcos contínuos distanciados de 20 cm.'
),
(
    'Cenoura',
    30,
    20,
    '{"min":400,"med":250,"max":100}',
    '{"min":150,"med":100,"max":50}',
    70,
    'Espaçamento de 0,3 x 0,03 m.'
),
(
    'Chuchu',
    20,
    20,
    '{"min":120,"med":80,"max":40}',
    '{"min":60,"med":40,"max":20}',
    80,
    'Espaçamentos 4,0 x 4,0 m.'
),
(
    'Citrus',
    NULL,
    350,
    '{"min":140,"med":90,"max":45}',
    '{"min":240,"med":170,"max":90}',
    70,
    'Espaçamento de 6 m x 4 m.'
),
(
    'Consórcio Milho x Feijão',
    30,
    20,
    '{"min":40,"med":20,"max":0}',
    '{"min":40,"med":20,"max":0}',
    70,
    'Fazer a inoculação do feijão com rizóbio.'
),
(
    'Coqueiro',
    NULL,
    300,
    '{"min":200,"med":150,"max":100}',
    '{"min":500,"med":400,"max":300}',
    60,
    'Utilize o sulfato de amônio ou superfosfato simples como fonte de N e P.'
),
(
    'Couve',
    200,
    40,
    '{"min":200,"med":140,"max":80}',
    '{"min":120,"med":80,"max":40}',
    80,
    'Espaçamento 0,8 x 0,5 m.'
),
(
    'Couve-flor',
    40,
    40,
    '{"min":200,"med":140,"max":80}',
    '{"min":120,"med":80,"max":40}',
    80,
    'Espaçamento 0,8 x 0,5 m.'
),
(
    'Espécies Nativas da Caatinga',
    NULL,
    130,
    '{"min":80,"med":80,"max":80}',
    '{"min":30,"med":30,"max":30}',
    80,
    'Espaçamento: 1,0 x 0,5 m.'
),
(
    'Eucalipito',
    10,
    10,
    '{"min":30,"med":20,"max":10}',
    '{"min":15,"med":10,"max":5}',
    70,
    'Espaçamento no campo: 2,0 x 3,0 m.'
),
(
    'Feijão',
    20,
    20,
    '{"min":40,"med":20,"max":0}',
    '{"min":60,"med":40,"max":0}',
    70,
    'Espaçamento 0,50 m entrelinhas e 10 sementes por metro de sulco.'
),
(
    'Fumo',
    35,
    20,
    '{"min":80,"med":60,"max":40}',
    '{"min":65,"med":50,"max":35}',
    50,
    'Espaçamento: 1,0 x 0,5 m.'
),
(
    'Goiaba',
    80,
    40,
    '{"min":80,"med":60,"max":40}',
    '{"min":60,"med":40,"max":30}',
    70,
    'Espaçamento de 6,0 x 6,0 m.'
),
(
    'Graviola',
    100,
    50,
    '{"min":80,"med":60,"max":40}',
    '{"min":60,"med":40,"max":20}',
    70,
    'Espaçamento de 5,0 x 6,0 m.'
),
(
    'Hortaliças',
    40,
    40,
    '{"min":160,"med":120,"max":80}',
    '{"min":160,"med":120,"max":80}',
    80,
    'Espaçamentos: 0,2 x 0,1 m.'
),
(
    'Inhame',
    20,
    20,
    '{"min":60,"med":40,"max":20}',
    '{"min":20,"med":15,"max":10}',
    50,
    'Espaçamento: 1,2 x 0,4 m.'
),
(
    'Leucena',
    NULL,
    10,
    '{"min":60,"med":40,"max":30}',
    '{"min":30,"med":20,"max":15}',
    70,
    'Espaçamento 3,0 x 3,0 m.'
),
(
    'Mamona',
    40,
    20,
    '{"min":80,"med":60,"max":40}',
    '{"min":60,"med":40,"max":20}',
    60,
    'Espaçamento: 2,5 x 2,0 m.'
),
(
    'Mamão',
    NULL,
    120,
    '{"min":100,"med":60,"max":30}',
    '{"min":120,"med":80,"max":50}',
    60,
    'Espaçamento das fileiras de 3,0 m x 2,0 m.'
),
(
    'Mandioca',
    30,
    30,
    '{"min":60,"med":40,"max":20}',
    '{"min":60,"med":40,"max":20}',
    60,
    'Espaçamento 1,0 x 0,6 m.'
),
(
    'Manga',
    80,
    60,
    '{"min":150,"med":100,"max":50}',
    '{"min":60,"med":40,"max":20}',
    60,
    NULL
),
(
    'Maracujá',
    NULL,
    100,
    '{"min":80,"med":60,"max":0}',
    '{"min":120,"med":90,"max":60}',
    50,
    'Espaçamento entre plantas de 3,0 a 5,0 m.'
),
(
    'Maracujá',
    NULL,
    100,
    '{"min":80,"med":60,"max":0}',
    '{"min":120,"med":90,"max":60}',
    50,
    'Espaçamento entre plantas de 3,0 a 5,0 m.'
),
(
    'Mata Virgem',
    NULL,
    0,
    '{"min":0,"med":0,"max":0}',
    '{"min":0,"med":0,"max":0}',
    0,
    NULL
),
(
    'Melancia',
    50,
    40,
    '{"min":160,"med":90,"max":60}',
    '{"min":120,"med":90,"max":60}',
    70,
    'Espaçamento 3,0 x 1,0 m.'
),
(
    'Melão',
    50,
    40,
    '{"min":160,"med":120,"max":80}',
    '{"min":160,"med":120,"max":80}',
    80,
    'Espaçamento de 2,0 x 0,5 m.'
),
(
    'Milho',
    70,
    30,
    '{"min":80,"med":60,"max":20}',
    '{"min":60,"med":40,"max":20}',
    80,
    'Espaçamento de 0,8 x 0,4 m.'
),
(
    'Palma Forrageira',
    NULL,
    130,
    '{"min":80,"med":80,"max":80}',
    '{"min":30,"med":30,"max":30}',
    80,
    'Espaçamento: 1,0 x 0,25 m.'
),
(
    'Pastagem',
    50,
    80,
    '{"min":80,"med":60,"max":20}',
    '{"min":60,"med":40,"max":20}',
    70,
    'Espaçamento entre sulcos de 0,9 m.'
),
(
    'Pastagem',
    50,
    80,
    '{"min":80,"med":60,"max":20}',
    '{"min":60,"med":40,"max":20}',
    70,
    'Espaçamento entre sulcos de 0,9 m.'
),
(
    'Pepino',
    30,
    40,
    '{"min":140,"med":80,"max":40}',
    '{"min":80,"med":60,"max":40}',
    80,
    'Espaçamento de 1,0 x 0,5 m.'
),
(
    'Pimentão',
    NULL,
    40,
    '{"min":160,"med":100,"max":40}',
    '{"min":120,"med":80,"max":40}',
    80,
    'Espaçamento: 1,0 x 0,4 m.'
),
(
    'Pinha',
    80,
    40,
    '{"min":80,"med":60,"max":40}',
    '{"min":60,"med":40,"max":30}',
    70,
    'Espaçamento 5,0 x 5,0 m.'
),
(
    'Quiabo',
    100,
    20,
    '{"min":159,"med":100,"max":70}',
    '{"min":50,"med":40,"max":20}',
    70,
    'Espaçamento 1,0 x 0,4 m.'
),
(
    'Repolho',
    50,
    50,
    '{"min":200,"med":100,"max":50}',
    '{"min":100,"med":50,"max":30}',
    70,
    'Espaçamento de 0,8 x 0,4 m.'
),
(
    'Sapoti',
    NULL,
    80,
    '{"min":120,"med":100,"max":80}',
    '{"min":80,"med":60,"max":30}',
    70,
    'Espaçamento de 8,0 x 8,0 m.'
),
(
    'Seriguela',
    NULL,
    60,
    '{"min":90,"med":60,"max":40}',
    '{"min":60,"med":40,"max":20}',
    60,
    'Espaçamento de 4,0 x 4,0 m.'
),
(
    'Sisal',
    NULL,
    100,
    '{"min":80,"med":60,"max":40}',
    '{"min":90,"med":75,"max":60}',
    50,
    'Espaçamento de 1,50 x 0,9 m.'
),
(
    'Sorgo',
    40,
    20,
    '{"min":80,"med":60,"max":40}',
    '{"min":60,"med":40,"max":20}',
    70,
    'Espaçamento: 1,0 m entre fileiras.'
),
(
    'Tomate',
    40,
    100,
    '{"min":600,"med":400,"max":200}',
    '{"min":200,"med":150,"max":100}',
    80,
    'Espaçamento de 1 x 0,5 m.'
),
(
    'Urucum',
    NULL,
    15,
    '{"min":40,"med":30,"max":20}',
    '{"min":60,"med":40,"max":0}',
    80,
    'Espaçamento: 4,0 x 4,0 m.'
);

BEGIN;

UPDATE parameters.parameter     2,
    'Hidrogênio + Alumínio',
    'cmol<sub>c</sub>/kg',
    2,
    'MAQS-Embrapa',
    NULL,
    'x*1.65'  
),