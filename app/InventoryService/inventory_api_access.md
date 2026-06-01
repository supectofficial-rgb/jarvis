# Inventory API Access Reference

This file is the quick reference for reaching the live Inventory and Auth APIs through the correct gateway path.

## Auth

- Base host: `http://194.5.188.40:5194`
- Login endpoint: `POST /api/UserService/Auth/login/by-credential`
- Required header: `X-Api-Key: ABC`
- Content type: `application/json`

Example request body:

```json
{
  "UserName": "NIVAD-ADMIN",
  "Password": "1qaz!QAZ"
}
```

The login response returns a JWT token and an expiration timestamp.

## Gateway

- Gateway host: `https://gateway.salarsanatnivad.com`
- Health check: `GET /health`

All inventory API calls should go through the gateway unless a direct upstream test is specifically needed.

## Inventory Service Prefix

- Base path through gateway: `/api/InventoryService`

Examples:

- `GET /api/InventoryService/ProductVariant/search`
- `GET /api/InventoryService/ProductVariant/{productVariantBusinessKey}/components`
- `PUT /api/InventoryService/ProductVariant/{productVariantBusinessKey}/components`

## Standard Headers For Protected Inventory Calls

- `Authorization: Bearer <jwt>`
- `Accept: application/json`
- `Content-Type: application/json` for write requests
- `X-Requested-With: XMLHttpRequest` for UI-originated AJAX calls

## Variant Component Write Contract

The component-upsert endpoint is:

- `PUT /api/InventoryService/ProductVariant/{productVariantBusinessKey}/components`

Body:

```json
{
  "ComponentVariantRef": "0bbf840e-2eda-49de-9b39-03d58f2beb62",
  "Quantity": 1
}
```

Optional field:

- `VariantComponentBusinessKey`

## Observed Live Behavior

- `GET /health` on the gateway returned `200`.
- `GET /api/InventoryService/ProductVariant/search?...` returned `200` with JSON.
- `PUT /api/InventoryService/ProductVariant/{id}/components` reached the gateway, but the live upstream timed out with `504 Gateway Timeout`.

## Notes

- Prefer the gateway route for normal validation and integration testing.
- Use the direct upstream IP only when gateway behavior itself must be ruled out.
- If a live write returns HTML or timeout, treat it as an upstream/backend issue rather than a client contract issue until proven otherwise.
