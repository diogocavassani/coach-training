# Dominio - Treino

## Entidade principal
`SessaoDeTreino`

### Campos
- `Id`
- `AtletaId`
- `Data`
- `Tipo`
- `DuracaoMinutos`
- `DistanciaKm`
- `Rpe` (`Value Object`)

## Regras de validacao
- `AtletaId` obrigatorio (`Guid` nao vazio).
- `DuracaoMinutos` > 0.
- `DistanciaKm` >= 0.
- `Data` nao pode ser futura.
- `RPE` entre 1 e 10.

## Decisao tecnica
- O `RPE` foi mantido como Value Object para consolidar linguagem ubiqua e evitar valores fora do intervalo aceito.
- A carga e derivada por `DuracaoMinutos x RPE` e serve de base para metricas futuras (ex.: ACWR).
